using System.Text.Json;
using System.Text.Json.Serialization;

namespace ChemicalSDS.Models
{
    public static class Sec05SubsectionCodec
    {
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

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
            string.IsNullOrWhiteSpace(c.Sec05Subsections) ? "[]" : c.Sec05Subsections!;

        public static List<Sec01SubsectionItem> DefaultTemplate() =>
        [
            new()
            {
                Heading = "5.1 Extinguishing media",
                Fields =
                [
                    new Sec01SubsectionField
                    {
                        Label = "Suitable extinguishing agents:",
                        Description = "dry extinguishing agent, alcohol-resistant foam, carbon dioxide, water spray."
                    },
                    new Sec01SubsectionField
                    {
                        Label = "Unsuitable extinguishing agents:",
                        Description = "a direct water jet can spread the fire."
                    }
                ]
            },
            new()
            {
                Heading = "5.2 Special hazard arising from the substance or mixture",
                Description =
                    "Do not allow wastewater from firefighting to enter sewers, surface waters or groundwater."
            },
            new()
            {
                Heading = "5.3 Advice for fire-fighters",
                Description =
                    "Avoid inhalation of smoke and fumes. In case of insufficient ventilation, use breathing apparatus. Contaminated extinguishing water must not be allowed to enter the sewage system and must be disposed of in accordance with applicable environmental regulations."
            }
        ];

        

        private static void SetField(List<Sec01SubsectionField> fields, string labelPrefix, string? value)
        {
            if (string.IsNullOrWhiteSpace(value)) return;
            var field = fields.FirstOrDefault(f =>
                f.Label.TrimStart().StartsWith(labelPrefix, StringComparison.OrdinalIgnoreCase));
            if (field != null)
                field.Description = value.Trim();
        }

        
    }
}
