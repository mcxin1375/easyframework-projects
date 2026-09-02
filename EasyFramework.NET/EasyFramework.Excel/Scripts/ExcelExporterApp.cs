using ClosedXML.Excel;
using Newtonsoft.Json;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

internal static class ExcelExporterApp
{
    public static int Run(string[] args)
    {
        try
        {
            var options = CliOptions.Parse(args);
            if (options is null) return 2;
            var inputExists = Directory.Exists(options.InputDirectory);
            var inputSettingsPath = Path.Combine(options.InputDirectory, "#_Settings.xlsx");
            var defaultSettingsPath = Path.Combine(AppContext.BaseDirectory, "Settings", "#_Settings.xlsx");
            var settingsPath = inputExists && File.Exists(inputSettingsPath) ? inputSettingsPath : defaultSettingsPath;
            if (!File.Exists(settingsPath)) throw new ExportException($"Settings path does not exist: {settingsPath}");
            var settings = SettingsReader.Read(settingsPath);
            if (!inputExists) throw new ExportException($"Input path does not exist: {options.InputDirectory}");
            var model = new ExportModel(settings);
            var codeDirectory = options.CodeDirectory ?? ResolveOutputPath(options.InputDirectory, settings.OutputScriptPath, "Code");
            var dataDirectory = options.DataDirectory ?? ResolveOutputPath(options.InputDirectory, settings.OutputDataPath, "Data");
            var files = Directory.EnumerateFiles(options.InputDirectory, "*.xlsx", SearchOption.AllDirectories)
                .Where(p => !Path.GetFileName(p).StartsWith("~$", StringComparison.OrdinalIgnoreCase))
                .Where(p => !Path.GetFileName(p).StartsWith('#'))
                .Where(p => !Path.GetRelativePath(options.InputDirectory, p).Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar).SkipLast(1).Any(x => x.StartsWith('#')))
                .ToArray();
            Console.WriteLine($"Input: {options.InputDirectory}");
            Console.WriteLine($"Code output: {codeDirectory}");
            Console.WriteLine($"Data output: {dataDirectory}");
            Console.WriteLine($"Excel files: {files.Length}");
            foreach (var file in files)
            {
                try
                {
                    Console.WriteLine($"Processing: {file}");
                    using var workbook = new XLWorkbook(file);
                    EnumParser.Read(workbook, file, model);
                    TableParser.Read(workbook, file, model);
                    Console.WriteLine($"Completed: {file}");
                }
                catch (Exception ex) when (ex is not ExportException) { Console.Error.WriteLine($"WARN: skipped '{file}': {ex.Message}"); }
            }
            model.Validate();
            OutputWriter.ClearDirectory(codeDirectory);
            OutputWriter.ClearDirectory(dataDirectory);
            CodeGenerator.Write(codeDirectory, model);
            DataGenerator.Write(dataDirectory, model, options.BinaryOverride ?? settings.Binary);
            Console.WriteLine($"Exported {model.Tables.Count} tables and {model.Enums.Count} enums.");
            return 0;
        }
        catch (ExportException ex) { Console.Error.WriteLine("ERROR: " + ex.Message); return 1; }
        catch (Exception ex) { Console.Error.WriteLine("ERROR: " + ex); return 1; }
    }
    private static string ResolveOutputPath(string inputDirectory, string? configuredPath, string fallbackName) => string.IsNullOrWhiteSpace(configuredPath) ? Path.Combine(inputDirectory, fallbackName) : Path.GetFullPath(Path.Combine(inputDirectory, configuredPath));
}
