using System.Text.Json;
using System.Text.Json.Serialization;

namespace ChemicalSDS.Models
{
    public sealed class Sec01SubsectionField
    {
        [JsonPropertyName("label")]
        public string Label { get; set; } = "";

        [JsonPropertyName("description")]
        public string Description { get; set; } = "";
    }

    public sealed class Sec01SubsectionItem
    {
        [JsonPropertyName("heading")]
        public string Heading { get; set; } = "";

        [JsonPropertyName("description")]
        public string Description { get; set; } = "";

        /// <summary>Optional sub-headings under this subsection (e.g. Manufacturer, Australian supplier).</summary>
        [JsonPropertyName("fields")]
        public List<Sec01SubsectionField>? Fields { get; set; }
    }

    public static class Sec01SubsectionCodec
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

        public static List<Sec01SubsectionItem> DefaultTemplate() =>
        [
            new() { Heading = "1.1 Product identifier", Description = "" },
            new() { Heading = "1.2 Relevant identified uses and uses advised against", Description = "" },
            new()
            {
                Heading = "1.3 Details of the supplier of the safety data sheet",
                Fields =
                [
                    new Sec01SubsectionField { Label = "Manufacturer:", Description = "" },
                    new Sec01SubsectionField { Label = "Australian supplier / importer:", Description = "" }
                ]
            },
            new() { Heading = "1.4 Emergency telephone number", Description = "" }
        ];

        public static string FromLegacy(Chemical c)
        {
            var items = DefaultTemplate();
            items[0].Description = c.ProductIdentifier ?? "";
            items[1].Description = c.ProductUse ?? "";
            var sec13Fields = items[2].Fields;
            if (sec13Fields is { Count: >= 2 })
            {
                sec13Fields[0].Description = c.Sec01Manufacturer ?? "";
                sec13Fields[1].Description = c.SupplierDetails ?? "";
            }
            items[3].Description = c.EmergencyPhone ?? "";
            return Serialize(items);
        }

        public static string ResolveStoredJson(Chemical c) =>
            string.IsNullOrWhiteSpace(c.Sec01Subsections) ? FromLegacy(c) : c.Sec01Subsections!;

        public static void SyncLegacyFields(Chemical target, string? json)
        {
            var rows = Parse(json);
            if (rows.Count == 0)
                return;

            static string? FindDesc(IEnumerable<Sec01SubsectionItem> list, string prefix)
            {
                var row = list.FirstOrDefault(r =>
                    r.Heading.TrimStart().StartsWith(prefix, StringComparison.OrdinalIgnoreCase));
                if (row == null) return null;
                if (row.Fields is { Count: > 0 })
                    return null;
                return string.IsNullOrWhiteSpace(row.Description) ? null : row.Description.Trim();
            }

            var productId = FindDesc(rows, "1.1");
            if (!string.IsNullOrWhiteSpace(productId))
            {
                target.ProductIdentifier = productId;
                target.ChemicalName = string.IsNullOrWhiteSpace(target.ChemicalName)
                    ? productId
                    : target.ChemicalName;
            }

            target.ProductUse = FindDesc(rows, "1.2") ?? target.ProductUse;
            target.EmergencyPhone = FindDesc(rows, "1.4") ?? target.EmergencyPhone;

            var sec13 = rows.FirstOrDefault(r => r.Heading.TrimStart().StartsWith("1.3", StringComparison.OrdinalIgnoreCase));
            if (sec13?.Fields is { Count: > 0 })
            {
                foreach (var f in sec13.Fields)
                {
                    if (string.IsNullOrWhiteSpace(f.Label)) continue;
                    if (f.Label.Contains("Manufacturer", StringComparison.OrdinalIgnoreCase) &&
                        !f.Label.Contains("Australian", StringComparison.OrdinalIgnoreCase))
                        target.Sec01Manufacturer = f.Description?.Trim();
                    else if (f.Label.Contains("Australian supplier", StringComparison.OrdinalIgnoreCase) ||
                             f.Label.Contains("importer", StringComparison.OrdinalIgnoreCase))
                        target.SupplierDetails = f.Description?.Trim();
                }
            }
            else
            {
                var sec13Text = FindDesc(rows, "1.3");
                if (!string.IsNullOrWhiteSpace(sec13Text))
                    target.SupplierDetails = sec13Text;
            }
        }

        public static bool HasFieldContent(Sec01SubsectionItem row) =>
            row.Fields != null && row.Fields.Any(f =>
                !string.IsNullOrWhiteSpace(f.Label) || !string.IsNullOrWhiteSpace(f.Description));
    }
}
