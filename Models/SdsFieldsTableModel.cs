namespace ChemicalSDS.Models
{
    public sealed class SdsFieldsTableModel
    {
        public string Column1Header { get; set; } = "Item";
        public string Column2Header { get; set; } = "Details";
        public IReadOnlyList<Sec01SubsectionField> Fields { get; set; } = [];
    }

    public static class SdsSubsectionTableHeaders
    {
        public static (string Col1, string Col2) ForHeading(string? heading)
        {
            var h = heading?.TrimStart() ?? "";
            if (h.StartsWith("16.1", StringComparison.OrdinalIgnoreCase))
                return ("H-phrase", "Full text");
            if (h.StartsWith("16.2", StringComparison.OrdinalIgnoreCase))
                return ("Classification", "Full text");
            if (h.StartsWith("16.3", StringComparison.OrdinalIgnoreCase))
                return ("Abbreviation", "Definition");
            if (h.StartsWith("9.1", StringComparison.OrdinalIgnoreCase))
                return ("Property", "Value / limits");
            if (h.StartsWith("11.1", StringComparison.OrdinalIgnoreCase))
                return ("Endpoint", "Information");
            if (h.StartsWith("4.1", StringComparison.OrdinalIgnoreCase))
                return ("Route / topic", "First aid");
            return ("Item", "Details");
        }
    }
}
