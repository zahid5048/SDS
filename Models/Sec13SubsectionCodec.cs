using System.Text.Json;
using System.Text.Json.Serialization;

namespace ChemicalSDS.Models
{
    public static class Sec13SubsectionCodec
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
            string.IsNullOrWhiteSpace(c.Sec13Subsections) ? "[]" : c.Sec13Subsections!;

        public static List<Sec01SubsectionItem> DefaultTemplate() =>
        [
            new()
            {
                Heading = "13.1 Waste treatment methods",
                Description =
                    "Do not dispose of residues or waste of the product via sewers or wastewater — treat in accordance with the instructions in Sections 6, 7 and 8. Do not reuse empty containers and do not incinerate. Hand over residual/waste product and contaminated packaging to an authorised waste disposal contractor. Dispose of in accordance with applicable Commonwealth, state and territory environmental regulations — confirm requirements with the relevant state or territory EPA."
            }
        ];

        

        
    }
}
