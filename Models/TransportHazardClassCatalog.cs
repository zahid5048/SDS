namespace ChemicalSDS.Models
{
    public static class TransportHazardClassCatalog
    {
        public record TransportClassItem(string Number, string Label, string ImagePath);

        public static readonly IReadOnlyList<TransportClassItem> All =
        [
            new("1", "Class 1 — Explosives", "/images/ghs/GHS01.svg"),
            new("2", "Class 2 — Gases", "/images/ghs/GHS04.svg"),
            new("3", "Class 3 — Flammable liquids", "/images/transport/class-3.svg"),
            new("4", "Class 4 — Flammable solids", "/images/ghs/GHS02.svg"),
            new("5", "Class 5 — Oxidizers", "/images/ghs/GHS03.svg"),
            new("6", "Class 6 — Toxic", "/images/ghs/GHS06.svg"),
            new("7", "Class 7 — Radioactive", "/images/transport/class-7.svg"),
            new("8", "Class 8 — Corrosives", "/images/ghs/GHS05.svg"),
            new("9", "Class 9 — Miscellaneous", "/images/ghs/GHS07.svg")
        ];

        public static string? GetImagePath(string? classNumber)
        {
            if (string.IsNullOrWhiteSpace(classNumber)) return null;
            var n = classNumber.Trim().Split(' ')[0];
            return All.FirstOrDefault(c => c.Number == n)?.ImagePath;
        }

        public static Dictionary<string, string> ParseCustomImages(string? json)
        {
            if (string.IsNullOrWhiteSpace(json)) return new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            try
            {
                return System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(json)
                    ?? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            }
            catch
            {
                return new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            }
        }
    }
}
