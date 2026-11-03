using System.Text.Json;
using System.Text.Json.Serialization;

namespace ChemicalSDS.Models
{
    public static class Sec11SubsectionCodec
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
            string.IsNullOrWhiteSpace(c.Sec11Subsections) ? "[]" : c.Sec11Subsections!;

        public static List<Sec01SubsectionItem> DefaultTemplate() =>
        [
            new()
            {
                Heading = "11.1 Information on hazard classes according to the GHS",
                Fields =
                [
                    new Sec01SubsectionField
                    {
                        Label = "Acute toxicity:",
                        Description = "The classification criteria are not met."
                    },
                    new Sec01SubsectionField
                    {
                        Label = "Skin corrosion/irritation:",
                        Description = "Causes skin irritation."
                    },
                    new Sec01SubsectionField
                    {
                        Label = "Serious eye damage / irritation:",
                        Description = "Causes serious eye irritation."
                    },
                    new Sec01SubsectionField
                    {
                        Label = "Respiratory or skin sensitisation:",
                        Description = "May cause an allergic skin reaction."
                    },
                    new Sec01SubsectionField { Label = "CMR effects:", Description = "" },
                    new Sec01SubsectionField
                    {
                        Label = "Germ cell mutagenicity:",
                        Description = "the classification criteria are not met."
                    },
                    new Sec01SubsectionField
                    {
                        Label = "Carcinogenicity:",
                        Description =
                            "the classification criteria are not met at product level. One ingredient (CAS 93-29-8) carries an AU Carcinogenicity Category 2 classification, but is present below the 1.0% GHS concentration limit for that classification to apply to the mixture."
                    },
                    new Sec01SubsectionField
                    {
                        Label = "Reproductive toxicity:",
                        Description = "the classification criteria are not met."
                    },
                    new Sec01SubsectionField
                    {
                        Label = "Specific target organ toxicity — single exposure:",
                        Description = "The classification criteria are not met."
                    },
                    new Sec01SubsectionField
                    {
                        Label = "Specific target organ toxicity — repeated exposure:",
                        Description =
                            "The classification criteria are not met at product level. One ingredient (CAS 80-56-8) carries an AU HCIS STOT RE 2 classification, but is present below the 1.0% GHS concentration limit for that classification to apply to the mixture."
                    },
                    new Sec01SubsectionField
                    {
                        Label = "Aspiration hazard:",
                        Description = "The classification criteria are not met."
                    }
                ]
            },
            new()
            {
                Heading = "11.2 Information on other hazards",
                Description = "No data available."
            }
        ];

        

        private static string? JoinNonEmpty(string? a, string? b, string sep)
        {
            var parts = new[] { a?.Trim(), b?.Trim() }.Where(s => !string.IsNullOrWhiteSpace(s)).ToList();
            return parts.Count == 0 ? null : string.Join(sep, parts);
        }

        

        private static void SetField(List<Sec01SubsectionField> fields, string labelPart, string? value)
        {
            if (string.IsNullOrWhiteSpace(value)) return;
            var field = fields.FirstOrDefault(f =>
                f.Label.Contains(labelPart, StringComparison.OrdinalIgnoreCase));
            if (field != null)
                field.Description = value.Trim();
        }

        
    }
}
