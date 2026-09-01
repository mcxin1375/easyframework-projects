using ClosedXML.Excel;
using Newtonsoft.Json;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

return ExcelExporterApp.Run(args);

internal static class ExcelExporterApp
{
    public static int Run(string[] args)
    {
        try
        {
            var options = CliOptions.Parse(args);
            if (options is null) return 2;
            var settingsPath = Path.Combine(options.InputDirectory, "#_Settings.xlsx");
            if (!File.Exists(settingsPath)) throw new ExportException($"Missing required file: {settingsPath}");
            var model = new ExportModel(SettingsReader.Read(settingsPath));
            var files = Directory.EnumerateFiles(options.InputDirectory, "*.xlsx", SearchOption.AllDirectories)
                .Where(p => !Path.GetFileName(p).StartsWith("~$", StringComparison.OrdinalIgnoreCase))
                .Where(p => Path.GetFileName(p).StartsWith("#Enum", StringComparison.OrdinalIgnoreCase) || !Path.GetFileName(p).StartsWith('#'))
                .Where(p => !Path.GetRelativePath(options.InputDirectory, p).Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar).SkipLast(1).Any(x => x.StartsWith('#')))
                .ToArray();
            foreach (var file in files)
            {
                try
                {
                    using var workbook = new XLWorkbook(file);
                    if (Path.GetFileName(file).StartsWith("#Enum", StringComparison.OrdinalIgnoreCase)) EnumParser.Read(workbook, file, model);
                    else TableParser.Read(workbook, file, model);
                }
                catch (Exception ex) when (ex is not ExportException) { Console.Error.WriteLine($"WARN: skipped '{file}': {ex.Message}"); }
            }
            model.Validate();
            OutputWriter.ClearDirectory(options.CodeDirectory);
            OutputWriter.ClearDirectory(options.DataDirectory);
            CodeGenerator.Write(options.CodeDirectory, model);
            DataGenerator.Write(options.DataDirectory, model, options.Binary);
            Console.WriteLine($"Exported {model.Tables.Count} tables and {model.Enums.Count} enums.");
            return 0;
        }
        catch (ExportException ex) { Console.Error.WriteLine("ERROR: " + ex.Message); return 1; }
        catch (Exception ex) { Console.Error.WriteLine("ERROR: " + ex); return 1; }
    }
}

internal sealed record CliOptions(string InputDirectory, string CodeDirectory, string DataDirectory, bool Binary)
{
    public static CliOptions? Parse(string[] args)
    {
        var values = args.Where(a => !a.Equals("--binary", StringComparison.OrdinalIgnoreCase)).ToList();
        var binary = args.Any(a => a.Equals("--binary", StringComparison.OrdinalIgnoreCase));
        if (values.Count == 0)
        {
            Console.Write("Excel directory: "); var input = Console.ReadLine();
            Console.Write("Code output directory: "); var code = Console.ReadLine();
            Console.Write("Data output directory: "); var data = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(input) || string.IsNullOrWhiteSpace(code) || string.IsNullOrWhiteSpace(data)) return null;
            values.AddRange([input, code, data]);
        }
        if (values.Count != 3) { Console.Error.WriteLine("Usage: EasyFramework.Excel <excel-dir> <code-dir> <data-dir> [--binary]"); return null; }
        var inputDir = Path.GetFullPath(values[0]);
        if (!Directory.Exists(inputDir)) throw new ExportException($"Input directory does not exist: {inputDir}");
        var codeDir = Path.GetFullPath(values[1]); var dataDir = Path.GetFullPath(values[2]);
        if (IsInside(inputDir, codeDir) || IsInside(inputDir, dataDir)) throw new ExportException("Output directories must not be inside the input directory.");
        if (codeDir.Equals(dataDir, StringComparison.OrdinalIgnoreCase)) throw new ExportException("Code and data output directories must be different.");
        return new(inputDir, codeDir, dataDir, binary);
    }
    private static bool IsInside(string parent, string candidate) => candidate.Equals(parent, StringComparison.OrdinalIgnoreCase) || candidate.StartsWith(parent.TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase);
}

internal sealed class Settings
{
    public string Namespace { get; init; } = "Game";
    public string ClassPrefix { get; init; } = "";
    public string ClassSuffix { get; init; } = "";
    public int RowDesc { get; init; }
    public int RowKey { get; init; }
    public int RowType { get; init; }
    public int RowCS { get; init; }
    public int RowDataIndex { get; init; }
    public Dictionary<string, TypeSpec> Types { get; } = new(StringComparer.OrdinalIgnoreCase);
}
internal sealed record TypeSpec(string Name, string CsType, string DefaultValue);

internal static class SettingsReader
{
    public static Settings Read(string path)
    {
        using var book = new XLWorkbook(path);
        var sheet = book.Worksheets.FirstOrDefault(s => s.Name.Equals("Settings", StringComparison.OrdinalIgnoreCase)) ?? throw new ExportException("Settings sheet is missing.");
        var map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var row in sheet.RowsUsed()) { var key = row.Cell(1).GetString().Trim(); if (key.Length > 0 && !key.Equals("Key", StringComparison.OrdinalIgnoreCase)) map[key] = row.Cell(2).GetString().Trim(); }
        int Required(string key) => int.TryParse(map.GetValueOrDefault(key), out var n) && n > 0 ? n : throw new ExportException($"Invalid setting '{key}'.");
        var result = new Settings { Namespace = map.GetValueOrDefault("Namespace") ?? "Game", ClassPrefix = map.GetValueOrDefault("ClassPrefix") ?? "", ClassSuffix = map.GetValueOrDefault("ClassSuffix") ?? "", RowDesc = Required("RowDesc"), RowKey = Required("RowKey"), RowType = Required("RowType"), RowCS = Required("RowCS"), RowDataIndex = Required("RowDataIndex") };
        var readme = book.Worksheets.FirstOrDefault(s => s.Name.Equals("Readme", StringComparison.OrdinalIgnoreCase)) ?? throw new ExportException("Readme sheet is missing.");
        foreach (var row in readme.RowsUsed()) { var name = row.Cell(1).GetString().Trim(); var value = row.Cell(2).GetString().Trim(); if (name.Length == 0 || name.Equals("Key", StringComparison.OrdinalIgnoreCase) || value.Length == 0) continue; var cs = value.Split('=', 2)[0].Trim(); result.Types[name] = new TypeSpec(name, cs, DefaultFor(cs)); }
        if (result.Types.Count == 0) throw new ExportException("Readme has no types.");
        return result;
    }
    private static string DefaultFor(string type) => type.ToLowerInvariant() switch { "string" => "string.Empty", "bool" => "false", "float" => "0f", "double" => "0d", _ => "0" };
}

internal sealed class ExportModel(Settings settings)
{
    public Settings Settings { get; } = settings;
    public List<TableModel> Tables { get; } = [];
    public List<EnumModel> Enums { get; } = [];
    public void Validate()
    {
        var errors = new List<string>();
        foreach (var g in Tables.GroupBy(t => t.ClassName, StringComparer.Ordinal)) if (g.Count() > 1) errors.Add($"Duplicate class '{g.Key}'.");
        foreach (var g in Enums.GroupBy(e => e.Name, StringComparer.Ordinal)) if (g.Count() > 1) errors.Add($"Duplicate enum '{g.Key}'.");
        foreach (var table in Tables) table.Validate(errors, Enums, Settings);
        if (errors.Count > 0) throw new ExportException("Validation failed:\n" + string.Join('\n', errors));
    }
}
internal sealed class TableModel
{
    public required string SourceFile { get; init; }
    public required string SheetName { get; init; }
    public required string ClassName { get; init; }
    public List<FieldModel> Fields { get; } = [];
    public List<Dictionary<string, object?>> Rows { get; } = [];
    public void Validate(List<string> errors, List<EnumModel> enums, Settings settings)
    {
        if (Fields.Count == 0) return;
        if (!Fields[0].IsClient) errors.Add($"{SourceFile}!{SheetName}: first column must contain C in RowCS.");
        var keys = new HashSet<string>(); foreach (var row in Rows) if (!keys.Add(Convert.ToString(row[Fields[0].CodeName], CultureInfo.InvariantCulture) ?? "")) errors.Add($"{SourceFile}!{SheetName}: duplicate primary key.");
        foreach (var f in Fields.Where(f => f.CsType.StartsWith("enum=", StringComparison.OrdinalIgnoreCase))) if (!enums.Any(e => e.Name.Equals(f.CsType[5..], StringComparison.OrdinalIgnoreCase))) errors.Add($"{SourceFile}!{SheetName}: unknown enum '{f.CsType[5..]}'.");
    }
}
internal sealed record FieldModel(string Name, string CodeName, string CsType, string Description, bool IsClient, int Column);
internal sealed class EnumModel { public required string Name { get; init; } public List<EnumItem> Items { get; } = []; }
internal sealed record EnumItem(string Name, int Value, string Description);

internal static class TableParser
{
    public static void Read(XLWorkbook book, string file, ExportModel model)
    {
        foreach (var sheet in book.Worksheets)
        {
            var s = model.Settings; var rows = sheet.RowsUsed().ToDictionary(r => r.RowNumber());
            if (!rows.ContainsKey(s.RowKey) || !rows.ContainsKey(s.RowType) || !rows.ContainsKey(s.RowCS)) continue;
            var table = new TableModel { SourceFile = file, SheetName = sheet.Name, ClassName = s.ClassPrefix + sheet.Name + s.ClassSuffix };
            var maxColumn = new[] { s.RowKey, s.RowType, s.RowCS }.Select(r => rows[r].LastCellUsed()?.Address.ColumnNumber ?? 0).Max();
            for (var col = 1; col <= maxColumn; col++)
            {
                var name = rows[s.RowKey].Cell(col).GetString().Trim(); var type = rows[s.RowType].Cell(col).GetString().Trim(); var cs = rows[s.RowCS].Cell(col).GetString().Trim();
                if (name.Length == 0 || type.Length == 0 || !cs.Contains('C', StringComparison.OrdinalIgnoreCase)) continue;
                var code = Sanitize(name); table.Fields.Add(new(name, code, type, rows.GetValueOrDefault(s.RowDesc)?.Cell(col).GetString() ?? "", true, col));
            }
            if (table.Fields.Count == 0) continue;
            foreach (var row in sheet.RowsUsed().Where(r => r.RowNumber() >= s.RowDataIndex))
            {
                if (row.CellsUsed().All(c => string.IsNullOrWhiteSpace(c.GetString()))) continue;
                var data = new Dictionary<string, object?>(); foreach (var field in table.Fields) data[field.CodeName] = ConvertValue(row.Cell(field.Column).GetString(), field.CsType);
                table.Rows.Add(data);
            }
            model.Tables.Add(table);
        }
    }
    private static string Sanitize(string value) => Regex.IsMatch(value, "^[A-Za-z_][A-Za-z0-9_]*$") ? value : Regex.Replace(value, "[^A-Za-z0-9_]", "_");
    private static object? ConvertValue(string value, string type)
    {
        if (type.StartsWith("enum=", StringComparison.OrdinalIgnoreCase)) return value.Length == 0 ? "0" : value;
        try { return type.ToLowerInvariant() switch { "string" => value, "bool" => bool.TryParse(value, out var b) ? b : value == "1", "byte" => byte.Parse(value.Length == 0 ? "0" : value), "short" => short.Parse(value.Length == 0 ? "0" : value), "ushort" => ushort.Parse(value.Length == 0 ? "0" : value), "int" => int.Parse(value.Length == 0 ? "0" : value), "uint" => uint.Parse(value.Length == 0 ? "0" : value), "long" => long.Parse(value.Length == 0 ? "0" : value), "ulong" => ulong.Parse(value.Length == 0 ? "0" : value), "float" => float.Parse(value.Length == 0 ? "0" : value, CultureInfo.InvariantCulture), "double" => double.Parse(value.Length == 0 ? "0" : value, CultureInfo.InvariantCulture), _ => value }; } catch (Exception ex) { throw new ExportException($"Invalid value '{value}' for type '{type}': {ex.Message}"); }
    }
}

internal static class EnumParser
{
    public static void Read(XLWorkbook book, string file, ExportModel model)
    {
        foreach (var sheet in book.Worksheets)
        {
            EnumModel? current = null;
            foreach (var row in sheet.RowsUsed())
            {
                var enumName = row.Cell(1).GetString().Trim();
                if (enumName.Equals("Enum", StringComparison.OrdinalIgnoreCase) || enumName.Equals("Key", StringComparison.OrdinalIgnoreCase)) continue;
                if (enumName.Length > 0) { current = new EnumModel { Name = enumName }; model.Enums.Add(current); }
                if (current is null) continue;
                var member = row.Cell(2).GetString().Trim().TrimEnd(',').Trim();
                if (member.Length == 0) continue;
                var value = int.TryParse(row.Cell(3).GetString(), out var n) ? n : current.Items.Count == 0 ? 0 : current.Items[^1].Value + 1;
                current.Items.Add(new(member, value, row.Cell(1).GetString()));
            }
        }
    }
}

internal static class CodeGenerator
{
    public static void Write(string directory, ExportModel model)
    {
        var ns = model.Settings.Namespace; Directory.CreateDirectory(directory);
        File.WriteAllText(Path.Combine(directory, "Enums.g.cs"), $"namespace {ns};\n\n" + string.Join("\n", model.Enums.Select(e => $"public enum {e.Name} {{ {string.Join(", ", e.Items.Select(i => $"{i.Name} = {i.Value}"))} }}")));
        foreach (var table in model.Tables)
        {
            var fields = string.Join("\n", table.Fields.Select(f => $"    public readonly {CsType(f.CsType)} {f.CodeName}; // {f.Description}"));
            var tableParameters = string.Join(", ", table.Fields.Select(f => $"{CsType(f.CsType)} {f.CodeName}"));
            var tableAssignments = string.Join("\n", table.Fields.Select(f => $"        this.{f.CodeName} = {f.CodeName};"));
            File.WriteAllText(Path.Combine(directory, table.ClassName + ".g.cs"), $"using Newtonsoft.Json;\nnamespace {ns};\n\npublic partial class {table.ClassName}\n{{\n{fields}\n    [JsonConstructor]\n    public {table.ClassName}({tableParameters})\n    {{\n{tableAssignments}\n    }}\n}}\n");
        }
        var declarations = string.Join("\n", model.Tables.Select(t => $"    public readonly {t.ClassName}[] {t.ClassName}Items;"));
        var parameters = string.Join(", ", model.Tables.Select(t => $"{t.ClassName}[] {t.ClassName}Items"));
        var assignments = string.Join("\n", model.Tables.Select(t => $"        this.{t.ClassName}Items = {t.ClassName}Items;"));
        var json = string.Join(", ", model.Tables.Select(t => $"root[\"{t.ClassName}Items\"]?.ToObject<{t.ClassName}[]>() ?? Array.Empty<{t.ClassName}>()"));
        var loader = $"using System;\nusing System.IO;\nusing System.Text;\nusing Newtonsoft.Json.Linq;\nnamespace {ns};\n\npublic sealed class ExcelData\n{{\n{declarations}\n    private ExcelData({parameters})\n    {{\n{assignments}\n    }}\n    private static ExcelData FromJson(string text) {{ var root = JObject.Parse(text); return new({json}); }}\n    public static ExcelData LoadJson(string file) => FromJson(File.ReadAllText(file));\n    public static ExcelData LoadBinary(string file) {{ using var reader = new BinaryReader(File.OpenRead(file)); var magic = Encoding.ASCII.GetString(reader.ReadBytes(9)); if (magic != \"EASYEXCEL\") throw new InvalidDataException(\"Invalid magic.\"); if (reader.ReadInt32() != 1 || reader.ReadInt32() != {model.Tables.Count}) throw new InvalidDataException(\"Unsupported binary format.\"); var length = reader.ReadInt32(); if (length < 0 || length > reader.BaseStream.Length - reader.BaseStream.Position) throw new InvalidDataException(\"Invalid payload length.\"); return FromJson(Encoding.UTF8.GetString(reader.ReadBytes(length))); }}\n}}\n";
        File.WriteAllText(Path.Combine(directory, "ExcelData.g.cs"), loader);
    }
    private static string CsType(string type) => type.StartsWith("enum=", StringComparison.OrdinalIgnoreCase) ? type[5..] : type;
}

internal static class DataGenerator
{
    public static void Write(string directory, ExportModel model, bool binary)
    {
        Directory.CreateDirectory(directory); var root = model.Tables.ToDictionary(t => t.ClassName + "Items", t => t.Rows); File.WriteAllText(Path.Combine(directory, "Data.json"), JsonConvert.SerializeObject(root, Formatting.Indented));
        if (!binary) return;
        var payload = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(root));
        using var writer = new BinaryWriter(File.Create(Path.Combine(directory, "Data.bytes")), Encoding.UTF8, false); writer.Write(Encoding.ASCII.GetBytes("EASYEXCEL")); writer.Write(1); writer.Write(model.Tables.Count); writer.Write(payload.Length); writer.Write(payload);
    }
}
internal static class OutputWriter { public static void ClearDirectory(string path) { Directory.CreateDirectory(path); foreach (var file in Directory.EnumerateFiles(path)) File.Delete(file); foreach (var dir in Directory.EnumerateDirectories(path)) Directory.Delete(dir, true); } }
internal sealed class ExportException(string message) : Exception(message);
