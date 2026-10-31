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
            string.IsNullOrWhiteSpace(c.Sec11Subsections) ? FromLegacy(c) : c.Sec11Subsections!;

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

        public static string FromLegacy(Chemical c)
        {
            var items = DefaultTemplate();
            var f = items[0].Fields!;

            SetField(f, "Acute toxicity", c.AcuteToxicity);
            if (!string.IsNullOrWhiteSpace(c.IrritationCorrosion))
            {
                if (c.IrritationCorrosion.Contains('|'))
                    SetField(f, "Skin corrosion", "See ingredient irritation/corrosion data on file.");
                else
                {
                    SetField(f, "Skin corrosion", c.IrritationCorrosion);
                    SetField(f, "Serious eye", c.IrritationCorrosion);
                }
            }
            SetField(f, "sensitisation", c.Sensitization);
            SetField(f, "Germ cell", c.Mutagenicity);
            var carc = JoinNonEmpty(c.Carcinogenicity, c.CarcinogenicityClassification, "\n");
            SetField(f, "Carcinogenicity:", carc);
            SetField(f, "Reproductive toxicity", c.ReproductiveToxicity ?? c.Teratogenicity);
            SetField(f, "single exposure", c.TargetOrganToxicitySingle);
            SetField(f, "repeated exposure", c.TargetOrganToxicityRepeated);
            SetField(f, "Aspiration", c.AspirationHazard);

            var extra = CollectLegacyNotes(c);
            if (!string.IsNullOrWhiteSpace(extra))
            {
                items.Add(new Sec01SubsectionItem
                {
                    Heading = "Additional toxicological information (legacy)",
                    Description = extra
                });
            }

            return Serialize(items);
        }

        private static string? JoinNonEmpty(string? a, string? b, string sep)
        {
            var parts = new[] { a?.Trim(), b?.Trim() }.Where(s => !string.IsNullOrWhiteSpace(s)).ToList();
            return parts.Count == 0 ? null : string.Join(sep, parts);
        }

        private static string? CollectLegacyNotes(Chemical c)
        {
            var parts = new List<string>();
            void Add(string label, string? val)
            {
                if (!string.IsNullOrWhiteSpace(val))
                    parts.Add($"{label}: {val.Trim()}");
            }
            Add("Routes of exposure", c.RoutesOfExposure);
            Add("Acute toxicity estimates", c.AcuteToxicityEstimates);
            Add("Chronic health effects", c.ChronicHealthEffects);
            return parts.Count == 0 ? null : string.Join("\n\n", parts);
        }

        private static void SetField(List<Sec01SubsectionField> fields, string labelPart, string? value)
        {
            if (string.IsNullOrWhiteSpace(value)) return;
            var field = fields.FirstOrDefault(f =>
                f.Label.Contains(labelPart, StringComparison.OrdinalIgnoreCase));
            if (field != null)
                field.Description = value.Trim();
        }

        public static void SyncLegacyFields(Chemical target, string? json)
        {
            var rows = Parse(json);
            if (rows.Count == 0)
                return;

            static string? GetField(Sec01SubsectionItem? row, string labelPart)
            {
                if (row?.Fields == null) return null;
                var f = row.Fields.FirstOrDefault(x =>
                    x.Label.Contains(labelPart, StringComparison.OrdinalIgnoreCase));
                return string.IsNullOrWhiteSpace(f?.Description) ? null : f.Description.Trim();
            }

            var sec111 = rows.FirstOrDefault(r =>
                r.Heading.TrimStart().StartsWith("11.1", StringComparison.OrdinalIgnoreCase));

            target.AcuteToxicity = GetField(sec111, "Acute toxicity") ?? target.AcuteToxicity;
            target.IrritationCorrosion =
                GetField(sec111, "Skin corrosion") ?? target.IrritationCorrosion;
            target.Sensitization = GetField(sec111, "sensitisation") ?? target.Sensitization;
            target.Mutagenicity = GetField(sec111, "Germ cell") ?? target.Mutagenicity;
            target.Carcinogenicity = GetField(sec111, "Carcinogenicity:") ?? target.Carcinogenicity;
            target.ReproductiveToxicity =
                GetField(sec111, "Reproductive toxicity") ?? target.ReproductiveToxicity;
            target.TargetOrganToxicitySingle =
                GetField(sec111, "single exposure") ?? target.TargetOrganToxicitySingle;
            target.TargetOrganToxicityRepeated =
                GetField(sec111, "repeated exposure") ?? target.TargetOrganToxicityRepeated;
            target.AspirationHazard = GetField(sec111, "Aspiration") ?? target.AspirationHazard;
        }
    }
}
