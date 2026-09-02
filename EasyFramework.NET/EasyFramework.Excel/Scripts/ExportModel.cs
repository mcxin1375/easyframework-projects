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
        if (!Fields[0].IsClient) errors.Add($"{SourceFile} | {SheetName}!{Fields[0].Column}: first column must contain C in RowCS.");
        var keys = new HashSet<string>(); foreach (var row in Rows) if (!keys.Add(Convert.ToString(row[Fields[0].CodeName], CultureInfo.InvariantCulture) ?? "")) errors.Add($"{SourceFile}!{SheetName}: duplicate primary key.");
        foreach (var f in Fields.Where(f => f.CsType.StartsWith("enum=", StringComparison.OrdinalIgnoreCase))) if (!enums.Any(e => e.Name.Equals(f.CsType[5..], StringComparison.OrdinalIgnoreCase))) errors.Add($"{SourceFile} | {SheetName}!{f.Column}: unknown enum '{f.CsType[5..]}'.");
    }
}
internal sealed record FieldModel(string Name, string CodeName, string CsType, string Description, bool IsClient, int Column);
internal sealed class EnumModel { public required string Name { get; init; } public List<EnumItem> Items { get; } = []; }
internal sealed record EnumItem(string Name, int Value, string Description);

internal sealed class ExportException(string message) : Exception(message);
