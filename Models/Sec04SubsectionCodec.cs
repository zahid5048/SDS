using System.Text.Json;
using System.Text.Json.Serialization;

namespace ChemicalSDS.Models
{
    /// <summary>Section 4 subsections (4.1–4.3); reuses <see cref="Sec01SubsectionItem"/> shape.</summary>
    public static class Sec04SubsectionCodec
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
            string.IsNullOrWhiteSpace(c.Sec04Subsections) ? "[]" : c.Sec04Subsections!;

        public static List<Sec01SubsectionItem> DefaultTemplate() =>
        [
            new()
            {
                Heading = "4.1 Description of first aid measures",
                Fields =
                [
                    new Sec01SubsectionField
                    {
                        Label = "General advice:",
                        Description = "Seek medical advice and show this safety data sheet."
                    },
                    new Sec01SubsectionField
                    {
                        Label = "Inhalation:",
                        Description =
                            "Remove affected person to fresh air and if unconscious place in recovery position. Seek medical attention for safety reasons."
                    },
                    new Sec01SubsectionField
                    {
                        Label = "Skin contact:",
                        Description =
                            "Wash with plenty of soap and water. Remove contaminated clothing and clean before reuse."
                    },
                    new Sec01SubsectionField
                    {
                        Label = "Eye contact:",
                        Description =
                            "Immediately remove contact lenses and rinse eyes thoroughly under running water with eyelids open (15 minutes). Seek medical attention if signs of tissue damage or irritation persist."
                    },
                    new Sec01SubsectionField
                    {
                        Label = "Ingestion:",
                        Description =
                            "Keep airways clear, do not administer milk or alcoholic beverages. Never administer anything orally to an unconscious person. If symptoms persist, consult a physician."
                    }
                ]
            },
            new()
            {
                Heading = "4.2 Most important symptoms and effects, both acute and delayed",
                Description = "No further relevant information available."
            },
            new()
            {
                Heading = "4.3 Indication of immediate medical attention and special treatment needed",
                Description =
                    "The Poisons Information Centre (13 11 26) is available nationally for treatment advice."
            }
        ];

        

        
    }
}
