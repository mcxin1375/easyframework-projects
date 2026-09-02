internal static class CodeGenerator
{
    public static void Write(string directory, ExportModel model)
    {
        var ns = model.Settings.Namespace; Directory.CreateDirectory(directory);
        File.WriteAllText(Path.Combine(directory, "Table.Enum.g.cs"), $"namespace {ns}\n{{\n" + string.Join("\n", model.Enums.Select(e => $"    public enum {e.Name} {{ {string.Join(", ", e.Items.Select(i => $"{i.Name} = {i.Value}"))} }}")) + "\n}\n");
        var classes = new StringBuilder("using Newtonsoft.Json;\nnamespace " + ns + "\n{\n");
        foreach (var table in model.Tables)
        {
            var fields = string.Join("\n", table.Fields.Select(f => $"        public readonly {CsType(f.CsType)} {f.CodeName}; // {f.Description}"));
            var tableParameters = string.Join(", ", table.Fields.Select(f => $"{CsType(f.CsType)} {f.CodeName}"));
            var tableAssignments = string.Join("\n", table.Fields.Select(f => $"            this.{f.CodeName} = {f.CodeName};"));
            classes.Append($"    public partial class {table.ClassName}\n    {{\n{fields}\n        [JsonConstructor]\n        public {table.ClassName}({tableParameters})\n        {{\n{tableAssignments}\n        }}\n    }}\n");
        }
        classes.Append("}\n");
        File.WriteAllText(Path.Combine(directory, "Table.Class.g.cs"), classes.ToString());
        var declarations = string.Join("\n", model.Tables.Select(t => $"        public readonly Table{t.ClassName} {t.ClassName};"));
        var parameters = string.Join(", ", model.Tables.Select(t => $"Table{t.ClassName} {t.ClassName}"));
        var assignments = string.Join("\n", model.Tables.Select(t => $"            this.{t.ClassName} = {t.ClassName};"));
        var json = string.Join(", ", model.Tables.Select(t => $"new Table{t.ClassName}(root[\"{t.ClassName}Items\"]?.ToObject<{t.ClassName}[]>() ?? Array.Empty<{t.ClassName}>())"));
        var managers = string.Join("\n", model.Tables.Select(t => { var key = t.Fields[0]; return $"    public sealed class Table{t.ClassName}\n    {{\n        public readonly {t.ClassName}[] Items;\n        private readonly Dictionary<{CsType(key.CsType)}, {t.ClassName}> index;\n        public Table{t.ClassName}({t.ClassName}[] items)\n        {{\n            Items = items;\n            index = items.ToDictionary(item => item.{key.CodeName});\n        }}\n        public {t.ClassName}? Get({CsType(key.CsType)} {key.CodeName}) => index.GetValueOrDefault({key.CodeName});\n    }}"; }));
        var loader = """
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
namespace __NS__
{
__MANAGERS__

    public sealed class TableLoader
    {
__DECLARATIONS__
        public static TableLoader? Instance { get; private set; }

        private TableLoader(__PARAMETERS__)
        {
__ASSIGNMENTS__
        }
        public static void LoadInstanceFromFile(string file) { Instance = LoadFromFile(file); }
        public static void LoadInstanceFromText(string content) { Instance = LoadFromText(content); }
        internal static TableLoader LoadFromText(string text) { var root = JObject.Parse(text); return new(__JSON__); }
        internal static TableLoader LoadFromFile(string file) { var payload = File.ReadAllBytes(file); if (payload.Length >= 9 && Encoding.ASCII.GetString(payload, 0, 9) == "EASYEXCEL") { using var stream = new MemoryStream(payload); using var reader = new BinaryReader(stream); reader.ReadBytes(9); if (reader.ReadInt32() != 1 || reader.ReadInt32() != __COUNT__) throw new InvalidDataException("Unsupported binary format."); var length = reader.ReadInt32(); if (length < 0 || length > reader.BaseStream.Length - reader.BaseStream.Position) throw new InvalidDataException("Invalid payload length."); return LoadFromText(Encoding.UTF8.GetString(reader.ReadBytes(length))); } return LoadFromText(Encoding.UTF8.GetString(payload)); }
    }
}
""".Replace("__NS__", ns).Replace("__MANAGERS__", managers).Replace("__DECLARATIONS__", declarations).Replace("__PARAMETERS__", parameters).Replace("__ASSIGNMENTS__", assignments).Replace("__JSON__", json).Replace("__COUNT__", model.Tables.Count.ToString());
        File.WriteAllText(Path.Combine(directory, "TableLoader.g.cs"), loader);
    }
    private static string CsType(string type) => type.StartsWith("enum=", StringComparison.OrdinalIgnoreCase) ? type[5..] : type;
}

