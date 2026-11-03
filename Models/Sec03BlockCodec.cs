using System.Text.Json;
using System.Text.Json.Serialization;

namespace ChemicalSDS.Models
{
    public sealed class Sec03MixtureRow
    {
        [JsonPropertyName("cas")]
        public string Cas { get; set; } = "";

        [JsonPropertyName("chemicalName")]
        public string ChemicalName { get; set; } = "";

        [JsonPropertyName("proportion")]
        public string Proportion { get; set; } = "";

        [JsonPropertyName("classification")]
        public string Classification { get; set; } = "";

        [JsonPropertyName("source")]
        public string Source { get; set; } = "";
    }

    public sealed class Sec03Block
    {
        [JsonPropertyName("type")]
        public string Type { get; set; } = "text";

        [JsonPropertyName("heading")]
        public string Heading { get; set; } = "";

        [JsonPropertyName("description")]
        public string Description { get; set; } = "";

        [JsonPropertyName("rows")]
        public List<Sec03MixtureRow> Rows { get; set; } = [];
    }

    public static class Sec03BlockCodec
    {
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        public static List<Sec03Block> Parse(string? json)
        {
            if (string.IsNullOrWhiteSpace(json)) return [];
            try
            {
                return JsonSerializer.Deserialize<List<Sec03Block>>(json, JsonOptions) ?? [];
            }
            catch
            {
                return [];
            }
        }

        public static string Serialize(IEnumerable<Sec03Block> blocks) =>
            JsonSerializer.Serialize(blocks.ToList(), JsonOptions);

        public static string ResolveStoredJson(Chemical c) =>
            string.IsNullOrWhiteSpace(c.Sec03Blocks) ? "[]" : c.Sec03Blocks!;

        public static List<Sec03Block> DefaultTemplate() =>
        [
            new Sec03Block
            {
                Type = "text",
                Heading = "3.1 Substances",
                Description = "Not applicable — this product is a mixture."
            },
            new Sec03Block
            {
                Type = "text",
                Heading = "3.2 Mixtures",
                Description = "Ingredient disclosure prepared in accordance with Schedule 8 of the Work Health and Safety Regulations 2012. Proportions are expressed as the standard Schedule 8 concentration bands (<10%, 10–30%, 30–60%, >60%), or as a narrower range within one of these bands. Classification is taken from the Safe Work Australia HCIS harmonised list where an entry exists; otherwise the ingredient is self-classified under the GHS."
            },
            new Sec03Block
            {
                Type = "mixtureTable",
                Rows =
                [
                    new Sec03MixtureRow { Cas = "25265-71-8", ChemicalName = "3-(3-Hydroxypropoxy)propan-1-ol", Proportion = "25–50%", Classification = "", Source = "Self-classified" },
                    new Sec03MixtureRow { Cas = "54464-57-2", ChemicalName = "1-(2,3,8,8-Tetramethyl-1,3,4,5,6,7-hexahydronaphthalen-2-yl)ethanone", Proportion = "5-10%", Classification = "Skin Sens. 1B; H317", Source = "AU HCIS" }
                ]
            },
            new Sec03Block
            {
                Type = "footnote",
                Description = "CAS 93-29-8 carries an AU Carcinogenicity Category 2 classification (H351) at ingredient level. It is present at <1%, below the 1.0% generic GHS concentration limit for that classification to apply to the mixture, so it does not affect the product-level classification in Section 2.1."
            }
        ];

        

        
    }
}
