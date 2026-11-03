using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

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
                var plain = ToPlainText(productId);
                var label = ShortProductLabel(plain);
                if (!string.IsNullOrWhiteSpace(label))
                {
                    if (string.IsNullOrWhiteSpace(target.ChemicalName)
                        || target.ChemicalName == plain
                        || target.ChemicalName == target.ProductIdentifier)
                        target.ChemicalName = label;
                    target.ProductIdentifier = label;
                }
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

        public static bool LooksLikeRichText(string? value) =>
            !string.IsNullOrEmpty(value) &&
            Regex.IsMatch(value, @"</?(b|strong|i|em|u|s|strike|sub|sup|span|p|div|br|ul|ol|li|font|img|h[1-6]|table|thead|tbody|tfoot|tr|td|th)\b", RegexOptions.IgnoreCase);

        /// <summary>True when the body the user wrote already contains this heading, so it must not be printed again.</summary>
        public static bool HeadingAlreadyInDescription(string? heading, string? description)
        {
            if (string.IsNullOrWhiteSpace(heading) || string.IsNullOrWhiteSpace(description))
                return false;
            var plain = Regex.Replace(ToPlainText(description), @"\s+", " ").Trim();
            var head = Regex.Replace(heading, @"\s+", " ").Trim();
            return plain.Contains(head, StringComparison.OrdinalIgnoreCase);
        }

        public static string SanitizeRichText(string? html)
        {
            if (string.IsNullOrWhiteSpace(html)) return "";
            var cleaned = Regex.Replace(html, @"<!--.*?-->", "", RegexOptions.Singleline);
            cleaned = Regex.Replace(cleaned, @"<(script|style|iframe|object|embed|link|meta|svg|math)\b[^>]*>.*?</\1>", "", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            cleaned = Regex.Replace(cleaned, @"</?(script|style|iframe|object|embed|link|meta|svg|math)\b[^>]*>", "", RegexOptions.IgnoreCase);
            cleaned = Regex.Replace(cleaned, @"\s+on\w+\s*=\s*(""[^""]*""|'[^']*'|[^\s>]+)", "", RegexOptions.IgnoreCase);
            cleaned = Regex.Replace(cleaned, @"(href|src)\s*=\s*(""|')\s*javascript:[^""']*\2", "", RegexOptions.IgnoreCase);
            return cleaned;
        }

        public static List<string> ToPlainLines(string? value)
        {
            if (string.IsNullOrWhiteSpace(value)) return [];
            if (!value.Contains('<'))
                return value.Replace("\r\n", "\n").Replace("\u200b", "").Split('\n').ToList();

            var text = Regex.Replace(value, @"<table\b[^>]*>[\s\S]*?</table>", match =>
            {
                var rows = Regex.Matches(match.Value, @"<tr\b[^>]*>[\s\S]*?</tr>", RegexOptions.IgnoreCase);
                var lines = new List<string>();
                foreach (Match row in rows)
                {
                    var cells = Regex.Matches(row.Value, @"<t[dh]\b[^>]*>([\s\S]*?)</t[dh]>", RegexOptions.IgnoreCase);
                    var values = new List<string>();
                    foreach (Match cell in cells)
                    {
                        var inner = Regex.Replace(cell.Groups[1].Value, "<[^>]+>", " ");
                        inner = WebUtility.HtmlDecode(inner).Replace('\u00a0', ' ');
                        inner = Regex.Replace(inner, @"\s+", " ").Trim();
                        values.Add(inner);
                    }
                    if (values.Any(v => v.Length > 0))
                        lines.Add(string.Join(" | ", values));
                }
                return "\n" + string.Join("\n", lines) + "\n";
            }, RegexOptions.IgnoreCase);
            text = Regex.Replace(text, @"<br\s*/?>", "\n", RegexOptions.IgnoreCase);
            text = Regex.Replace(text, @"</(p|div|h[1-6]|tr)>", "\n", RegexOptions.IgnoreCase);
            text = Regex.Replace(text, @"<li[^>]*>", "\n• ", RegexOptions.IgnoreCase);
            text = Regex.Replace(text, "<[^>]+>", "");
            text = WebUtility.HtmlDecode(text).Replace('\u00a0', ' ').Replace("\u200b", "");
            return text.Replace("\r\n", "\n").Split('\n')
                .Select(line => Regex.Replace(line, @"[ \t]+", " ").Trim())
                .Where(line => line.Length > 0)
                .ToList();
        }

        public static string ToPlainText(string? value)
        {
            if (string.IsNullOrWhiteSpace(value)) return "";
            if (!value.Contains('<')) return value.Replace("\u200b", "").Trim();
            return string.Join(" ", ToPlainLines(value)).Trim();
        }

        public static string ShortProductLabel(string? value)
        {
            var text = Regex.Replace(ToPlainText(value), @"\s+", " ").Trim();
            if (text.Length == 0) return "";

            var numbered = Regex.Match(
                text,
                @"Product identifier\s*/\s*No\.?\s*:\s*(.+?)(?=\s+\d+\.\d+\b|$)",
                RegexOptions.IgnoreCase);
            if (numbered.Success)
            {
                var name = numbered.Groups[1].Value.Trim().Trim(' ', '.', ';', ':');
                if (name.Length > 0)
                    return name.Length > 80 ? name[..80].Trim() : name;
            }

            var stripped = Regex.Replace(text, @"^1\.1\s+Product identifier\s*", "", RegexOptions.IgnoreCase).Trim();
            var candidate = stripped.Length > 0 ? stripped : text;
            return candidate.Length <= 80 ? candidate : candidate[..77].Trim() + "...";
        }

        public static bool HasFieldContent(Sec01SubsectionItem row) =>
            row.Fields != null && row.Fields.Any(f =>
                !string.IsNullOrWhiteSpace(f.Label) || !string.IsNullOrWhiteSpace(f.Description));
    }
}
