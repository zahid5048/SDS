using System.Text.Json;
using System.Text.Json.Serialization;

namespace ChemicalSDS.Models
{
    public static class Sec06SubsectionCodec
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
            string.IsNullOrWhiteSpace(c.Sec06Subsections) ? "[]" : c.Sec06Subsections!;

        public static List<Sec01SubsectionItem> DefaultTemplate() =>
        [
            new()
            {
                Heading = "6.1 Personal precautions, protective equipment and emergency procedures",
                Fields =
                [
                    new Sec01SubsectionField
                    {
                        Label = "For non-emergency personnel:",
                        Description =
                            "Wear suitable protective gloves when handling spilled material. Do not smoke. Avoid open flames and other sources of ignition. Avoid contact with skin. Do not inhale vapours. Individual cleaning after possible contact. Ensure adequate ventilation."
                    },
                    new Sec01SubsectionField
                    {
                        Label = "For emergency personnel:",
                        Description = "Apply the same recommendations as for non-emergency personnel."
                    }
                ]
            },
            new()
            {
                Heading = "6.2 Environmental precautions",
                Description =
                    "Do not allow entry to drains, surface waters or groundwater. In case of contamination of water, inform the relevant environmental regulator (e.g. the state or territory EPA)."
            },
            new()
            {
                Heading = "6.3 Methods and material for containment and cleaning up",
                Fields =
                [
                    new Sec01SubsectionField
                    {
                        Label = "For containment:",
                        Description =
                            "Set up barriers and cover the sewage system to prevent large-scale spread."
                    },
                    new Sec01SubsectionField
                    {
                        Label = "For cleaning up:",
                        Description =
                            "Immediately absorb with suitable liquid-binding, non-flammable materials (e.g. sand, silica gel, universal binder). Collect bound material in suitable, sealable containers and dispose of in accordance with applicable regulations."
                    }
                ]
            },
            new()
            {
                Heading = "6.4 Reference to other sections",
                Description =
                    "See Section 7 for safe handling, Section 8 for personal protective equipment, and Section 13 for disposal."
            }
        ];

        

        private static void SetField(List<Sec01SubsectionField> fields, string labelPrefix, string? value)
        {
            if (string.IsNullOrWhiteSpace(value)) return;
            var field = fields.FirstOrDefault(f =>
                f.Label.TrimStart().StartsWith(labelPrefix, StringComparison.OrdinalIgnoreCase) ||
                f.Label.Contains(labelPrefix, StringComparison.OrdinalIgnoreCase));
            if (field != null)
                field.Description = value.Trim();
        }

        
    }
}
