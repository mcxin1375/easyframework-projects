using ClosedXML.Excel;

internal static class EnumParser
{
    public static void Read(XLWorkbook book, string file, ExportModel model)
    {
        foreach (var sheet in book.Worksheets)
        {
            if (!sheet.Name.Equals("Enum", StringComparison.OrdinalIgnoreCase)) continue;
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
