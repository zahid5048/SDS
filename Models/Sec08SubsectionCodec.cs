using System.Text.Json;
using System.Text.Json.Serialization;

namespace ChemicalSDS.Models
{
    public static class Sec08SubsectionCodec
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
            string.IsNullOrWhiteSpace(c.Sec08Subsections) ? FromLegacy(c) : c.Sec08Subsections!;

        public static List<Sec01SubsectionItem> DefaultTemplate() =>
        [
            new()
            {
                Heading = "8.1 Control parameters",
                Description =
                    "No Australian Workplace Exposure Limit (WEL) has been established for any ingredient in this mixture, based on the Safe Work Australia Workplace Exposure Limits list (amended November 2025)."
            },
            new()
            {
                Heading = "8.2 Exposure controls",
                Fields =
                [
                    new Sec01SubsectionField
                    {
                        Label = "Engineering measures:",
                        Description = "Ensure adequate ventilation, especially in confined areas."
                    },
                    new Sec01SubsectionField
                    {
                        Label = "Additional information about the design of technical facilities:",
                        Description = "no further data — see Section 7."
                    },
                    new Sec01SubsectionField
                    {
                        Label = "Personal protective equipment:",
                        Description = ""
                    },
                    new Sec01SubsectionField
                    {
                        Label = "Respiratory protection:",
                        Description =
                            "In case of insufficient ventilation, use a respirator selected and fitted in accordance with AS/NZS 1715 and AS/NZS 1716 (or other governmental standards as applicable)."
                    },
                    new Sec01SubsectionField
                    {
                        Label = "Gloves:",
                        Description =
                            "Wear solvent-resistant gloves selected in accordance with AS/NZS 2161.1. The permeation time must exceed the contact time."
                    },
                    new Sec01SubsectionField
                    {
                        Label = "Eye protection:",
                        Description =
                            "Use safety goggles and a face shield tested in accordance with AS/NZS 1336 and AS/NZS 1337."
                    },
                    new Sec01SubsectionField
                    {
                        Label = "Body protection:",
                        Description = "Wear work clothes that cover arms and legs."
                    },
                    new Sec01SubsectionField
                    {
                        Label = "Hygiene measures:",
                        Description =
                            "Avoid contact with skin and eyes. Do not eat, drink or smoke during use. Wash hands after use."
                    }
                ]
            },
            new()
            {
                Heading = "8.3 Limitation and supervision of exposure into the environment",
                Description =
                    "Take precautions to prevent the product from entering the sewage system. In case of contamination of water or the sewage system, inform the responsible environmental authorities."
            }
        ];

        public static string FromLegacy(Chemical c)
        {
            var items = DefaultTemplate();

            var oelRows = OelLimitsFormatter.Parse(c.OccupationalExposureLimits);
            if (oelRows.Count > 0)
            {
                var lines = oelRows
                    .Where(r => !string.IsNullOrWhiteSpace(r.Ingredient) || !string.IsNullOrWhiteSpace(r.Limits))
                    .Select(r => string.IsNullOrWhiteSpace(r.Ingredient)
                        ? r.Limits.Trim()
                        : $"{r.Ingredient.Trim()}: {r.Limits.Trim()}");
                items[0].Description = string.Join("\n", lines);
            }
            else if (!string.IsNullOrWhiteSpace(c.OccupationalExposureLimits) &&
                     !c.OccupationalExposureLimits.Contains(OelLimitsFormatter.RowSeparator, StringComparison.Ordinal))
            {
                items[0].Description = c.OccupationalExposureLimits.Trim();
            }

            var sec82 = items[1].Fields;
            if (sec82 != null)
            {
                SetField(sec82, "Engineering measures", c.ExposureControls);
                SetField(sec82, "Respiratory", c.RespiratoryProtection);
                SetField(sec82, "Gloves", c.HandProtection);
                SetField(sec82, "Eye protection", c.EyeProtection);
                SetField(sec82, "Body protection", c.BodyProtection);
                SetField(sec82, "Hygiene measures", c.HygieneMeasures);
                if (!string.IsNullOrWhiteSpace(c.OtherSkinProtection))
                {
                    var gloves = FieldDesc(items[1], "Gloves");
                    var extra = c.OtherSkinProtection.Trim();
                    SetField(sec82, "Gloves",
                        string.IsNullOrWhiteSpace(gloves) ? extra : gloves + "\n" + extra);
                }
            }

            if (!string.IsNullOrWhiteSpace(c.EnvironmentalExposureControls))
                items[2].Description = c.EnvironmentalExposureControls.Trim();

            return Serialize(items);
        }

        private static void SetField(List<Sec01SubsectionField>? fields, string labelPrefix, string? value)
        {
            if (fields == null || string.IsNullOrWhiteSpace(value)) return;
            var field = fields.FirstOrDefault(f =>
                f.Label.Contains(labelPrefix, StringComparison.OrdinalIgnoreCase));
            if (field != null)
                field.Description = value.Trim();
        }

        private static string? FieldDesc(Sec01SubsectionItem row, string labelPrefix)
        {
            if (row.Fields == null) return null;
            var f = row.Fields.FirstOrDefault(x =>
                x.Label.Contains(labelPrefix, StringComparison.OrdinalIgnoreCase));
            return string.IsNullOrWhiteSpace(f?.Description) ? null : f.Description.Trim();
        }

        public static void SyncLegacyFields(Chemical target, string? json)
        {
            var rows = Parse(json);
            if (rows.Count == 0)
                return;

            static string? FieldDesc(Sec01SubsectionItem? row, string labelPrefix)
            {
                if (row?.Fields == null) return null;
                var f = row.Fields.FirstOrDefault(x =>
                    x.Label.Contains(labelPrefix, StringComparison.OrdinalIgnoreCase));
                return string.IsNullOrWhiteSpace(f?.Description) ? null : f.Description.Trim();
            }

            var sec81 = rows.FirstOrDefault(r =>
                r.Heading.TrimStart().StartsWith("8.1", StringComparison.OrdinalIgnoreCase));
            if (!string.IsNullOrWhiteSpace(sec81?.Description))
                target.OccupationalExposureLimits = sec81.Description.Trim();

            var sec82 = rows.FirstOrDefault(r =>
                r.Heading.TrimStart().StartsWith("8.2", StringComparison.OrdinalIgnoreCase));

            target.ExposureControls = FieldDesc(sec82, "Engineering measures") ?? target.ExposureControls;
            target.RespiratoryProtection = FieldDesc(sec82, "Respiratory") ?? target.RespiratoryProtection;
            target.HandProtection = FieldDesc(sec82, "Gloves") ?? target.HandProtection;
            target.EyeProtection = FieldDesc(sec82, "Eye protection") ?? target.EyeProtection;
            target.BodyProtection = FieldDesc(sec82, "Body protection") ?? target.BodyProtection;
            target.HygieneMeasures = FieldDesc(sec82, "Hygiene measures") ?? target.HygieneMeasures;

            var sec83 = rows.FirstOrDefault(r =>
                r.Heading.TrimStart().StartsWith("8.3", StringComparison.OrdinalIgnoreCase));
            if (!string.IsNullOrWhiteSpace(sec83?.Description))
                target.EnvironmentalExposureControls = sec83.Description.Trim();
        }
    }
}
