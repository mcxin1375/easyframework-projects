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
            var settingsPath = Path.Combine(options.InputDirectory, "#_Settings.xlsx");
            if (!File.Exists(settingsPath))
            {
                var defaultSettingsPath = Path.Combine(AppContext.BaseDirectory, "Settings", "#_Settings.xlsx");
                if (!File.Exists(defaultSettingsPath)) throw new ExportException($"Default settings file does not exist: {defaultSettingsPath}");
                Directory.CreateDirectory(options.InputDirectory);
                File.Copy(defaultSettingsPath, settingsPath);
                Console.WriteLine($"Copied default settings to '{settingsPath}'.");
            }
            var settings = SettingsReader.Read(settingsPath);
            var model = new ExportModel(settings);
            var codeDirectory = options.CodeDirectory ?? ResolveOutputPath(options.InputDirectory, settings.OutputScriptPath, "Code");
            var dataDirectory = options.DataDirectory ?? ResolveOutputPath(options.InputDirectory, settings.OutputDataPath, "Data");
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
