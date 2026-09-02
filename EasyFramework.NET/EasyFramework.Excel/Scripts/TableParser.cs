using ClosedXML.Excel;

internal static class TableParser
{
    public static void Read(XLWorkbook book, string file, ExportModel model)
    {
        foreach (var sheet in book.Worksheets)
        {
            if (sheet.Name.Equals("Enum", StringComparison.OrdinalIgnoreCase)) continue;
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
                var data = new Dictionary<string, object?>(); foreach (var field in table.Fields)
                {
                    var cell = row.Cell(field.Column);
                    try { data[field.CodeName] = ConvertValue(cell.GetString(), field.CsType); }
                    catch (ExportException ex) { throw new ExportException($"{file} | {sheet.Name}!{cell.Address}: {ex.Message}"); }
                }
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
