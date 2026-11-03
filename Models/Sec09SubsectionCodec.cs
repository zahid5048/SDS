using System.Text.Json;
using System.Text.Json.Serialization;

namespace ChemicalSDS.Models
{
    public static class Sec09SubsectionCodec
    {
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        private const string NotDetermined = "Not determined";

        public static List<Sec01SubsectionItem> Parse(string? json)
        {
            if (string.IsNullOrWhiteSpace(json))
                return [];
            try
            {
                return JsonSerializer.Deserialize<List<Sec01SubsectionItem>>(json, JsonOptions) ?? [];
            }
            catch
            {
                return [];
            }
        }

        public static string Serialize(IEnumerable<Sec01SubsectionItem> items) =>
            JsonSerializer.Serialize(items.ToList(), JsonOptions);

        public static string ResolveStoredJson(Chemical c) =>
            string.IsNullOrWhiteSpace(c.Sec09Subsections) ? "[]" : c.Sec09Subsections!;

        public static List<Sec01SubsectionItem> DefaultTemplate() =>
        [
            new()
            {
                Heading = "9.1 Information on basic physical and chemical properties",
                Fields = DefaultPropertyFields()
            },
            new()
            {
                Heading = "9.2 Other information",
                Fields =
                [
                    new Sec01SubsectionField { Label = "Solvent content:", Description = "49.9%" },
                    new Sec01SubsectionField { Label = "VOC:", Description = "3.28%" }
                ]
            }
        ];

        private static List<Sec01SubsectionField> DefaultPropertyFields() =>
        [
            new() { Label = "State of aggregation:", Description = "Liquid" },
            new() { Label = "Colour:", Description = "Light yellow" },
            new() { Label = "Odour:", Description = "Fresh – woody – marine" },
            new() { Label = "Melting point / freezing point:", Description = NotDetermined },
            new() { Label = "Boiling point / boiling range:", Description = NotDetermined },
            new() { Label = "Flammability:", Description = NotDetermined },
            new() { Label = "Lower and upper explosion limits:", Description = NotDetermined },
            new() { Label = "Flash point:", Description = "80.0°C (method: Pensky-Martens closed cup)" },
            new() { Label = "Ignition temperature:", Description = NotDetermined },
            new() { Label = "Decomposition temperature:", Description = NotDetermined },
            new() { Label = "pH value:", Description = "Non-aqueous product" },
            new() { Label = "Kinematic viscosity:", Description = NotDetermined },
            new() { Label = "Solubility:", Description = NotDetermined },
            new() { Label = "Partition coefficient n-octanol/water (log value):", Description = NotDetermined },
            new() { Label = "Vapour pressure:", Description = NotDetermined },
            new() { Label = "Relative density:", Description = "0.9807" },
            new() { Label = "Relative vapour density:", Description = NotDetermined },
            new() { Label = "Particulate properties:", Description = NotDetermined }
        ];

        

        private static string? JoinNonEmpty(string? a, string? b, string sep)
        {
            var parts = new[] { a?.Trim(), b?.Trim() }.Where(s => !string.IsNullOrWhiteSpace(s)).ToList();
            return parts.Count == 0 ? null : string.Join(sep, parts);
        }

        private static string? LabelValue(string label, string? value) =>
            string.IsNullOrWhiteSpace(value) ? null : $"{label}: {value.Trim()}";

        private static void SetField(List<Sec01SubsectionField> fields, string labelPart, string? value)
        {
            if (string.IsNullOrWhiteSpace(value)) return;
            var field = fields.FirstOrDefault(f =>
                f.Label.Contains(labelPart, StringComparison.OrdinalIgnoreCase));
            if (field != null)
                field.Description = value.Trim();
        }

        private static string? FieldDesc(List<Sec01SubsectionField> fields, string labelPart)
        {
            var f = fields.FirstOrDefault(x =>
                x.Label.Contains(labelPart, StringComparison.OrdinalIgnoreCase));
            return string.IsNullOrWhiteSpace(f?.Description) ? null : f.Description.Trim();
        }

        
    }
}
