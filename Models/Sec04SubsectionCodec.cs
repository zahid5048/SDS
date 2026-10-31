using System.Text.Json;
using System.Text.Json.Serialization;

namespace ChemicalSDS.Models
{
    /// <summary>Section 4 subsections (4.1–4.3); reuses <see cref="Sec01SubsectionItem"/> shape.</summary>
    public static class Sec04SubsectionCodec
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
            string.IsNullOrWhiteSpace(c.Sec04Subsections) ? FromLegacy(c) : c.Sec04Subsections!;

        public static List<Sec01SubsectionItem> DefaultTemplate() =>
        [
            new()
            {
                Heading = "4.1 Description of first aid measures",
                Fields =
                [
                    new Sec01SubsectionField
                    {
                        Label = "General advice:",
                        Description = "Seek medical advice and show this safety data sheet."
                    },
                    new Sec01SubsectionField
                    {
                        Label = "Inhalation:",
                        Description =
                            "Remove affected person to fresh air and if unconscious place in recovery position. Seek medical attention for safety reasons."
                    },
                    new Sec01SubsectionField
                    {
                        Label = "Skin contact:",
                        Description =
                            "Wash with plenty of soap and water. Remove contaminated clothing and clean before reuse."
                    },
                    new Sec01SubsectionField
                    {
                        Label = "Eye contact:",
                        Description =
                            "Immediately remove contact lenses and rinse eyes thoroughly under running water with eyelids open (15 minutes). Seek medical attention if signs of tissue damage or irritation persist."
                    },
                    new Sec01SubsectionField
                    {
                        Label = "Ingestion:",
                        Description =
                            "Keep airways clear, do not administer milk or alcoholic beverages. Never administer anything orally to an unconscious person. If symptoms persist, consult a physician."
                    }
                ]
            },
            new()
            {
                Heading = "4.2 Most important symptoms and effects, both acute and delayed",
                Description = "No further relevant information available."
            },
            new()
            {
                Heading = "4.3 Indication of immediate medical attention and special treatment needed",
                Description =
                    "The Poisons Information Centre (13 11 26) is available nationally for treatment advice."
            }
        ];

        public static string FromLegacy(Chemical c)
        {
            var items = DefaultTemplate();
            var sec41 = items[0].Fields;
            if (sec41 == null)
                return Serialize(items);

            static void SetField(List<Sec01SubsectionField> fields, string labelPrefix, string? value)
            {
                if (string.IsNullOrWhiteSpace(value)) return;
                var field = fields.FirstOrDefault(f =>
                    f.Label.TrimStart().StartsWith(labelPrefix, StringComparison.OrdinalIgnoreCase));
                if (field != null)
                    field.Description = value.Trim();
            }

            SetField(sec41, "Inhalation", c.InhalationFirstAid);
            SetField(sec41, "Skin contact", c.SkinContactFirstAid);
            SetField(sec41, "Eye contact", c.EyeContactFirstAid);
            SetField(sec41, "Ingestion", c.IngestionFirstAid);

            if (!string.IsNullOrWhiteSpace(c.NotesToPhysician) ||
                !string.IsNullOrWhiteSpace(c.SpecificTreatments) ||
                !string.IsNullOrWhiteSpace(c.ProtectionOfFirstAiders))
            {
                var parts = new List<string>();
                if (!string.IsNullOrWhiteSpace(c.NotesToPhysician))
                    parts.Add(c.NotesToPhysician.Trim());
                if (!string.IsNullOrWhiteSpace(c.SpecificTreatments))
                    parts.Add(c.SpecificTreatments.Trim());
                if (!string.IsNullOrWhiteSpace(c.ProtectionOfFirstAiders))
                    parts.Add("Protection of first-aiders: " + c.ProtectionOfFirstAiders.Trim());
                items[1].Description = string.Join("\n\n", parts);
            }

            if (!string.IsNullOrWhiteSpace(c.MedicalAttentionIndication))
                items[2].Description = c.MedicalAttentionIndication.Trim();

            return Serialize(items);
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
                    x.Label.TrimStart().StartsWith(labelPrefix, StringComparison.OrdinalIgnoreCase));
                return string.IsNullOrWhiteSpace(f?.Description) ? null : f.Description.Trim();
            }

            var sec41 = rows.FirstOrDefault(r =>
                r.Heading.TrimStart().StartsWith("4.1", StringComparison.OrdinalIgnoreCase));

            target.InhalationFirstAid = FieldDesc(sec41, "Inhalation") ?? target.InhalationFirstAid;
            target.SkinContactFirstAid = FieldDesc(sec41, "Skin contact") ?? target.SkinContactFirstAid;
            target.EyeContactFirstAid = FieldDesc(sec41, "Eye contact") ?? target.EyeContactFirstAid;
            target.IngestionFirstAid = FieldDesc(sec41, "Ingestion") ?? target.IngestionFirstAid;

            var sec42 = rows.FirstOrDefault(r =>
                r.Heading.TrimStart().StartsWith("4.2", StringComparison.OrdinalIgnoreCase));
            if (!string.IsNullOrWhiteSpace(sec42?.Description))
                target.NotesToPhysician = sec42.Description.Trim();

            var sec43 = rows.FirstOrDefault(r =>
                r.Heading.TrimStart().StartsWith("4.3", StringComparison.OrdinalIgnoreCase));
            if (!string.IsNullOrWhiteSpace(sec43?.Description))
                target.MedicalAttentionIndication = sec43.Description.Trim();
        }
    }
}
