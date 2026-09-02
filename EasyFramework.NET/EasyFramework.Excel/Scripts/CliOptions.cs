internal sealed record CliOptions(string InputDirectory, string? CodeDirectory, string? DataDirectory, bool? BinaryOverride)
{
    public static CliOptions? Parse(string[] args)
    {
        var values = new List<string>();
        bool? binary = null;
        for (var i = 0; i < args.Length; i++)
        {
            if (args[i].Equals("--binary", StringComparison.OrdinalIgnoreCase)) { binary = true; continue; }
            if (args[i].Equals("--output-type", StringComparison.OrdinalIgnoreCase))
            {
                if (++i >= args.Length) { Console.Error.WriteLine("Missing value for --output-type."); return null; }
                binary = args[i].Equals("binary", StringComparison.OrdinalIgnoreCase) ? true : args[i].Equals("json", StringComparison.OrdinalIgnoreCase) ? false : null;
                if (binary is null) { Console.Error.WriteLine("Output type must be json or binary."); return null; }
                continue;
            }
            values.Add(args[i]);
        }
        if (values.Count == 0)
        {
            Console.Write("Excel directory: "); var input = Console.ReadLine();
            Console.Write("Code output directory: "); var code = Console.ReadLine();
            Console.Write("Data output directory: "); var data = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(input) || string.IsNullOrWhiteSpace(code) || string.IsNullOrWhiteSpace(data)) return null;
            values.AddRange([input, code, data]);
        }
        if (values.Count != 1 && values.Count != 3) { Console.Error.WriteLine("Usage: EasyFramework.Excel <excel-dir> [<code-dir> <data-dir>] [--output-type json|binary] [--binary]"); return null; }
        var inputDir = Path.GetFullPath(values[0]);
        if (values.Count == 3 && string.IsNullOrWhiteSpace(values[1])) throw new ExportException("Code output path is empty.");
        if (values.Count == 3 && string.IsNullOrWhiteSpace(values[2])) throw new ExportException("Data output path is empty.");
        var codeDir = values.Count == 3 ? Path.GetFullPath(values[1]) : null;
        var dataDir = values.Count == 3 ? Path.GetFullPath(values[2]) : null;
        if (codeDir is not null && (IsInside(inputDir, codeDir) || IsInside(inputDir, dataDir!))) throw new ExportException("Output directories must not be inside the input directory.");
        if (codeDir is not null && codeDir.Equals(dataDir, StringComparison.OrdinalIgnoreCase)) throw new ExportException("Code and data output directories must be different.");
        return new(inputDir, codeDir, dataDir, binary);
    }
    private static bool IsInside(string parent, string candidate) => candidate.Equals(parent, StringComparison.OrdinalIgnoreCase) || candidate.StartsWith(parent.TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase);
}
