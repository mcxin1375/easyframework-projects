internal static class DataGenerator
{
    public static void Write(string directory, ExportModel model, bool binary)
    {
        Directory.CreateDirectory(directory);
        var tables = model.Tables.Select(table => $"  {JsonConvert.SerializeObject(table.ClassName + "Items")}: [\n" + string.Join(",\n", table.Rows.Select(row => "    " + JsonConvert.SerializeObject(row, Formatting.None))) + "\n  ]");
        var root = model.Tables.ToDictionary(t => t.ClassName + "Items", t => t.Rows);
        if (!binary)
        {
            File.WriteAllText(Path.Combine(directory, model.Settings.OutputDataFileName), "{\n" + string.Join(",\n", tables) + "\n}\n", Encoding.UTF8);
            return;
        }
        var payload = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(root));
        using var writer = new BinaryWriter(File.Create(Path.Combine(directory, model.Settings.OutputDataFileName)), Encoding.UTF8, false); writer.Write(Encoding.ASCII.GetBytes("EASYEXCEL")); writer.Write(1); writer.Write(model.Tables.Count); writer.Write(payload.Length); writer.Write(payload);
    }
}

