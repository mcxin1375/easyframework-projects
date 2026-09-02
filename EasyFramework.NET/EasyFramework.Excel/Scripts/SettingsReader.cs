internal sealed class Settings
{
    public string Namespace { get; init; } = "Game";
    public string ClassPrefix { get; init; } = "";
    public string ClassSuffix { get; init; } = "";
    public bool Binary { get; init; }
    public string? OutputScriptPath { get; init; }
    public string? OutputDataPath { get; init; }
    public int RowDesc { get; init; }
    public int RowKey { get; init; }
    public int RowType { get; init; }
    public int RowCS { get; init; }
    public int RowDataIndex { get; init; }
}

internal static class SettingsReader
{
    public static Settings Read(string path)
    {
        XLWorkbook book;
        try { book = new XLWorkbook(path); } catch (Exception ex) { throw new ExportException($"{path}: cannot read workbook: {ex.Message}"); }
        using (book)
        {
        var sheet = book.Worksheets.FirstOrDefault(s => s.Name.Equals("Settings", StringComparison.OrdinalIgnoreCase)) ?? throw new ExportException($"{path} | Settings: worksheet is missing.");
        var map = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (var row in sheet.RowsUsed()) { var key = row.Cell(1).GetString().Trim(); if (key.Length > 0 && !key.Equals("Key", StringComparison.OrdinalIgnoreCase)) map[key] = row.Cell(2).GetString().Trim(); }
        int Required(string key) => int.TryParse(map.GetValueOrDefault(key), out var n) && n > 0 ? n : throw new ExportException($"{path} | Settings!{key}: invalid setting value.");
        var outputType = map.GetValueOrDefault("OutputDataType") ?? "0";
        var binaryOutput = outputType.Trim() == "1" || outputType.Contains("binary", StringComparison.OrdinalIgnoreCase) || outputType.Contains("二进制", StringComparison.OrdinalIgnoreCase);
        var result = new Settings { Namespace = map.GetValueOrDefault("Namespace") ?? "Game", ClassPrefix = map.GetValueOrDefault("ClassPrefix") ?? "", ClassSuffix = map.GetValueOrDefault("ClassSuffix") ?? "", Binary = binaryOutput, OutputScriptPath = map.GetValueOrDefault("OutputScriptPath"), OutputDataPath = map.GetValueOrDefault("OutputDataPath"), RowDesc = Required("RowDesc"), RowKey = Required("RowKey"), RowType = Required("RowType"), RowCS = Required("RowCS"), RowDataIndex = Required("RowDataIndex") };
        return result;
        }
    }
}
