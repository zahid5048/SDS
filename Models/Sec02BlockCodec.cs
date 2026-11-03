using System.Text.Json;
using System.Text.Json.Serialization;

namespace ChemicalSDS.Models
{
    public sealed class Sec02CasRow
    {
        [JsonPropertyName("cas")]
        public string Cas { get; set; } = "";

        [JsonPropertyName("chemicalName")]
        public string ChemicalName { get; set; } = "";
    }

    public sealed class Sec02Block
    {
        [JsonPropertyName("type")]
        public string Type { get; set; } = "text";

        [JsonPropertyName("heading")]
        public string Heading { get; set; } = "";

        [JsonPropertyName("description")]
        public string Description { get; set; } = "";

        [JsonPropertyName("title")]
        public string Title { get; set; } = "";

        [JsonPropertyName("codes")]
        public string Codes { get; set; } = "";

        [JsonPropertyName("blockId")]
        public string? BlockId { get; set; }

        [JsonPropertyName("customImagePath")]
        public string? CustomImagePath { get; set; }

        [JsonPropertyName("rows")]
        public List<Sec02CasRow> Rows { get; set; } = [];
    }

    public static class Sec02BlockCodec
    {
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        public static List<Sec02Block> Parse(string? json)
        {
            if (string.IsNullOrWhiteSpace(json))
                return [];
            try
            {
                return JsonSerializer.Deserialize<List<Sec02Block>>(json, JsonOptions) ?? [];
            }
            catch
            {
                return [];
            }
        }

        public static string Serialize(IEnumerable<Sec02Block> blocks) =>
            JsonSerializer.Serialize(blocks.ToList(), JsonOptions);

        public static string ResolveStoredJson(Chemical c) =>
            string.IsNullOrWhiteSpace(c.Sec02Blocks) ? "[]" : c.Sec02Blocks!;

        public static List<Sec02Block> DefaultTemplate() =>
        [
            new Sec02Block
            {
                Type = "text",
                Heading = "2.1 Classification of the substance or mixture",
                Description = "Classification per the GHS as adopted under Schedule 6 of the Work Health and Safety Regulations 2012:\n• GHS07 — Skin Irrit. 2, H315: Causes skin irritation\n• GHS07 — Skin Sens. 1, H317: May cause an allergic skin reaction\n• GHS07 — Eye Irrit. 2, H319: Causes serious eye irritation\n• GHS09 — Aquatic Chronic 2, H411: Toxic to aquatic life with long lasting effects"
            },
            new Sec02Block { Type = "text", Heading = "2.2 Label elements", Description = "" },
            new Sec02Block { Type = "pictograms", Codes = "GHS07,GHS09" },
            new Sec02Block { Type = "text", Heading = "Signal word", Description = "Warning" },
            new Sec02Block
            {
                Type = "text",
                Heading = "Hazard statements",
                Description = "• H315: Causes skin irritation.\n• H317: May cause an allergic skin reaction.\n• H319: Causes serious eye irritation.\n• H411: Toxic to aquatic life with long lasting effects."
            },
            new Sec02Block
            {
                Type = "text",
                Heading = "Precautionary statements — General",
                Description = "• P101: If medical advice is needed, have product container or label at hand.\n• P103: Read label before use."
            },
            new Sec02Block { Type = "text", Heading = "Precautionary statements — Prevention", Description = "" },
            new Sec02Block { Type = "text", Heading = "Precautionary statements — Response", Description = "" },
            new Sec02Block { Type = "text", Heading = "Precautionary statements — Storage", Description = "" },
            new Sec02Block { Type = "text", Heading = "Precautionary statements — Disposal", Description = "" },
            new Sec02Block
            {
                Type = "table",
                Title = "Hazardous component(s) for labelling:",
                Rows = [new Sec02CasRow(), new Sec02CasRow()]
            },
            new Sec02Block
            {
                Type = "table",
                Title = "EUH208 — Contains an ingredient that may produce an allergic reaction:",
                Rows = [new Sec02CasRow(), new Sec02CasRow()]
            },
            new Sec02Block
            {
                Type = "text",
                Heading = "2.3 Other hazards",
                Description = "This mixture does not contain any PBT (persistent, bioaccumulative and toxic) or vPvB (very persistent and very bioaccumulative) substances. This mixture does not contain endocrine disrupting substances. Handling may cause static charge which may ignite the mixture."
            }
        ];

        

        private static void SetTextDesc(List<Sec02Block> blocks, string heading, string? value)
        {
            if (string.IsNullOrWhiteSpace(value)) return;
            var block = blocks.FirstOrDefault(b =>
                b.Type == "text" && b.Heading.Equals(heading, StringComparison.OrdinalIgnoreCase));
            if (block != null) block.Description = value.Trim();
        }

        

        private static string? FindText(List<Sec02Block> blocks, string heading)
        {
            var b = blocks.FirstOrDefault(x =>
                x.Type == "text" && x.Heading.Equals(heading, StringComparison.OrdinalIgnoreCase));
            return string.IsNullOrWhiteSpace(b?.Description) ? null : b.Description.Trim();
        }

        public static string MergePictogramCodes(Chemical c)
        {
            var blocks = Parse(ResolveStoredJson(c));
            var fromBlocks = blocks.Where(b => b.Type == "pictograms" && !string.IsNullOrWhiteSpace(b.Codes))
                .SelectMany(b => b.Codes.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries));
            var set = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var code in fromBlocks) set.Add(code);
            return set.Count == 0 ? "" : string.Join(",", set);
        }
    }
}
