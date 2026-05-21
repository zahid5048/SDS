namespace ChemicalSDS.Models
{
    public static class GhsPictogramCatalog
    {
        public record GhsItem(string Code, string Label, string ImagePath);

        public static readonly IReadOnlyList<GhsItem> All =
        [
            new("GHS01", "Explosive", "/images/ghs/GHS01.svg"),
            new("GHS02", "Flammable", "/images/ghs/GHS02.svg"),
            new("GHS03", "Oxidizing", "/images/ghs/GHS03.svg"),
            new("GHS04", "Compressed gas", "/images/ghs/GHS04.svg"),
            new("GHS05", "Corrosive", "/images/ghs/GHS05.svg"),
            new("GHS06", "Toxic", "/images/ghs/GHS06.svg"),
            new("GHS07", "Harmful / irritant", "/images/ghs/GHS07.svg"),
            new("GHS08", "Health hazard", "/images/ghs/GHS08.svg"),
            new("GHS09", "Environmental", "/images/ghs/GHS09.svg")
        ];

        public static HashSet<string> ParseCodes(string? csv) =>
            string.IsNullOrWhiteSpace(csv)
                ? []
                : csv.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                    .ToHashSet(StringComparer.OrdinalIgnoreCase);
    }
}
