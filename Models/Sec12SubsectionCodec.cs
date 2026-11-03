using System.Text.Json;
using System.Text.Json.Serialization;

namespace ChemicalSDS.Models
{
    public static class Sec12SubsectionCodec
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
            string.IsNullOrWhiteSpace(c.Sec12Subsections) ? "[]" : c.Sec12Subsections!;

        public static List<Sec01SubsectionItem> DefaultTemplate() =>
        [
            new()
            {
                Heading = "12.1 Toxicity",
                Description =
                    "No data available for the mixture as a whole. The mixture is classified Aquatic Chronic 2, H411 (see Section 2.1)."
            },
            new()
            {
                Heading = "12.2 Persistence and degradability",
                Description = "No data available for the mixture."
            },
            new()
            {
                Heading = "12.3 Bioaccumulative potential",
                Description = "No data available for the mixture."
            },
            new()
            {
                Heading = "12.4 Mobility in soil",
                Description = "No data available for the mixture."
            },
            new()
            {
                Heading = "12.5 Results of PBT and vPvB assessment",
                Description = "This mixture does not contain any PBT or vPvB substances."
            },
            new()
            {
                Heading = "12.6 Endocrine disrupting properties",
                Description = "This mixture does not contain endocrine disrupting substances."
            },
            new()
            {
                Heading = "12.7 Other adverse effects",
                Description =
                    "In case of improper handling or disposal, an environmental hazard cannot be excluded."
            }
        ];

        

        private static string? FormatLegacyText(string? value, string? fallback)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;
            if (value.Contains('|'))
                return fallback;
            return value.Trim();
        }

        private static void SetDesc(List<Sec01SubsectionItem> items, string prefix, string? value)
        {
            if (string.IsNullOrWhiteSpace(value)) return;
            var row = items.FirstOrDefault(r =>
                r.Heading.TrimStart().StartsWith(prefix, StringComparison.OrdinalIgnoreCase));
            if (row != null)
                row.Description = value.Trim();
        }

        
    }
}
