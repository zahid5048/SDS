using System.Text.Json;
using System.Text.Json.Serialization;

namespace ChemicalSDS.Models
{
    public static class Sec16SubsectionCodec
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
            string.IsNullOrWhiteSpace(c.Sec16Subsections) ? "[]" : c.Sec16Subsections!;

        public static List<Sec01SubsectionItem> DefaultTemplate() =>
        [
            new()
            {
                Heading = "16.1 Full text of abbreviated H-phrases",
                Fields = DefaultHPhraseFields()
            },
            new()
            {
                Heading = "16.2 Full text of abbreviated classifications (GHS)",
                Fields = DefaultGhsFields()
            },
            new()
            {
                Heading = "16.3 Abbreviations and acronyms",
                Fields = DefaultAbbrevFields()
            },
            new()
            {
                Heading = "16.4 Recommended applications",
                Description = "Concentrated perfumery raw material. Unsuitable for personal use as such."
            },
            new()
            {
                Heading = "16.5 Revision",
                Fields =
                [
                    new Sec01SubsectionField { Label = "Revision date", Description = "" },
                    new Sec01SubsectionField { Label = "Revision No.", Description = "" },
                    new Sec01SubsectionField
                    {
                        Label = "Preparation note",
                        Description = "Prepared for the Australian market."
                    }
                ]
            }
        ];

        private static List<Sec01SubsectionField> DefaultHPhraseFields() =>
        [
            Field("H226", "Flammable liquid and vapour."),
            Field("H301", "Toxic if swallowed."),
            Field("H302", "Harmful if swallowed."),
            Field("H304", "May be fatal if swallowed and enters airways."),
            Field("H315", "Causes skin irritation."),
            Field("H317", "May cause an allergic skin reaction."),
            Field("H318", "Causes serious eye damage."),
            Field("H319", "Causes serious eye irritation."),
            Field("H330", "Fatal if inhaled."),
            Field("H332", "Harmful if inhaled."),
            Field("H336", "May cause drowsiness or dizziness."),
            Field("H351", "Suspected of causing cancer."),
            Field("H361", "Suspected of damaging fertility or the unborn child."),
            Field("H373", "May cause damage to organs through prolonged or repeated exposure."),
            Field("H400", "Very toxic to aquatic life."),
            Field("H410", "Very toxic to aquatic life with long lasting effects."),
            Field("H411", "Toxic to aquatic life with long lasting effects."),
            Field("H412", "Harmful to aquatic life with long lasting effects.")
        ];

        private static List<Sec01SubsectionField> DefaultGhsFields() =>
        [
            Field("Acute Tox.", "Acute Toxicity"),
            Field("Aquatic Acute/Chronic", "Acute/Chronic Aquatic Toxicity"),
            Field("Asp. Haz.", "Aspiration Hazard"),
            Field("Carc.", "Carcinogenicity"),
            Field("Eye Dam./Eye Irrit.", "Serious Eye Damage/Eye Irritation"),
            Field("Flam. Liq.", "Flammable Liquid"),
            Field("Repr.", "Reproductive Toxicity"),
            Field("Skin Irrit.", "Skin Irritation"),
            Field("Skin Sens.", "Skin Sensitisation"),
            Field("STOT RE/SE", "Specific Target Organ Toxicity, Repeated/Single Exposure")
        ];

        private static List<Sec01SubsectionField> DefaultAbbrevFields() =>
        [
            Field("ADG", "Australian Dangerous Goods Code"),
            Field("ADR", "European Agreement concerning the International Carriage of Dangerous Goods by Road"),
            Field("AICIS", "Australian Industrial Chemicals Introduction Scheme"),
            Field("GHS", "Globally Harmonized System of Classification and Labelling of Chemicals"),
            Field("HCIS", "Hazardous Chemical Information System (Safe Work Australia)"),
            Field("IATA", "International Air Transport Association"),
            Field("IMDG", "International Maritime Dangerous Goods Code"),
            Field("WEL", "Workplace Exposure Limit (Safe Work Australia)"),
            Field("WHS", "Work Health and Safety")
        ];

        private static Sec01SubsectionField Field(string label, string description) =>
            new() { Label = label, Description = description };

        

        

        public static string FormatHPhrase(Sec01SubsectionField f)
        {
            var code = f.Label.Trim().TrimEnd(':');
            var text = f.Description.Trim();
            if (string.IsNullOrWhiteSpace(code))
                return string.IsNullOrWhiteSpace(text) ? "" : "• " + text;
            if (string.IsNullOrWhiteSpace(text))
                return "• " + code;
            if (code.StartsWith('H') && !code.Contains(':'))
                return $"• {code}: {text.TrimEnd('.')}.";
            return $"• {code}: {text}";
        }

        public static string FormatGhs(Sec01SubsectionField f)
        {
            var left = f.Label.Trim().TrimEnd('.');
            var right = f.Description.Trim();
            if (string.IsNullOrWhiteSpace(left))
                return string.IsNullOrWhiteSpace(right) ? "" : "• " + right;
            if (string.IsNullOrWhiteSpace(right))
                return "• " + left;
            return $"• {left} = {right}";
        }

        public static string FormatAbbrev(Sec01SubsectionField f)
        {
            var code = f.Label.Trim().TrimEnd(':');
            var text = f.Description.Trim();
            if (string.IsNullOrWhiteSpace(code))
                return string.IsNullOrWhiteSpace(text) ? "" : "• " + text;
            if (string.IsNullOrWhiteSpace(text))
                return "• " + code;
            return $"• {code}: {text}";
        }

        public static string ComposeRevisionParagraph(Sec01SubsectionItem? s165)
        {
            string? date = null;
            string? rev = null;
            string? note = null;

            if (s165?.Fields is { Count: > 0 })
            {
                foreach (var f in s165.Fields)
                {
                    var val = f.Description?.Trim();
                    if (string.IsNullOrWhiteSpace(val)) continue;
                    if (f.Label.Contains("date", StringComparison.OrdinalIgnoreCase))
                        date = val;
                    else if (f.Label.Contains("Revision No", StringComparison.OrdinalIgnoreCase) ||
                             f.Label.Contains("No.", StringComparison.OrdinalIgnoreCase))
                        rev = val;
                    else
                        note = val;
                }
            }
            else if (!string.IsNullOrWhiteSpace(s165?.Description))
            {
                return s165.Description.Trim();
            }

            var parts = new List<string>();
            if (!string.IsNullOrWhiteSpace(date))
                parts.Add($"Revision date: {date.TrimEnd('.')}.");
            if (!string.IsNullOrWhiteSpace(rev))
                parts.Add($"Revision No.: {rev.TrimEnd('.')}.");
            if (!string.IsNullOrWhiteSpace(note))
            {
                var n = note.Trim();
                if (!n.EndsWith('.'))
                    n += ".";
                parts.Add(n);
            }

            return parts.Count == 0 ? "" : string.Join(' ', parts);
        }

        private static string JoinLines(IEnumerable<Sec01SubsectionField> fields, Func<Sec01SubsectionField, string> format) =>
            string.Join("\n", fields.Select(format).Where(s => !string.IsNullOrWhiteSpace(s)));

        private static List<string> ParseMultiline(string? text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return [];
            return text.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(l => l.TrimStart('•', ' ', '\t'))
                .Where(l => !string.IsNullOrWhiteSpace(l))
                .ToList();
        }

        private static Sec01SubsectionField ParseColonLine(string line)
        {
            var idx = line.IndexOf(':');
            if (idx <= 0)
                return new Sec01SubsectionField { Label = "", Description = line.Trim() };
            return new Sec01SubsectionField
            {
                Label = line[..idx].Trim(),
                Description = line[(idx + 1)..].Trim().TrimEnd('.')
            };
        }

        private static Sec01SubsectionField ParseEqualsLine(string line)
        {
            var idx = line.IndexOf('=');
            if (idx <= 0)
                return new Sec01SubsectionField { Label = "", Description = line.Trim() };
            return new Sec01SubsectionField
            {
                Label = line[..idx].Trim(),
                Description = line[(idx + 1)..].Trim()
            };
        }
    }
}
