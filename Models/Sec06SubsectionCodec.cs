using System.Text.Json;
using System.Text.Json.Serialization;

namespace ChemicalSDS.Models
{
    public static class Sec06SubsectionCodec
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
            string.IsNullOrWhiteSpace(c.Sec06Subsections) ? FromLegacy(c) : c.Sec06Subsections!;

        public static List<Sec01SubsectionItem> DefaultTemplate() =>
        [
            new()
            {
                Heading = "6.1 Personal precautions, protective equipment and emergency procedures",
                Fields =
                [
                    new Sec01SubsectionField
                    {
                        Label = "For non-emergency personnel:",
                        Description =
                            "Wear suitable protective gloves when handling spilled material. Do not smoke. Avoid open flames and other sources of ignition. Avoid contact with skin. Do not inhale vapours. Individual cleaning after possible contact. Ensure adequate ventilation."
                    },
                    new Sec01SubsectionField
                    {
                        Label = "For emergency personnel:",
                        Description = "Apply the same recommendations as for non-emergency personnel."
                    }
                ]
            },
            new()
            {
                Heading = "6.2 Environmental precautions",
                Description =
                    "Do not allow entry to drains, surface waters or groundwater. In case of contamination of water, inform the relevant environmental regulator (e.g. the state or territory EPA)."
            },
            new()
            {
                Heading = "6.3 Methods and material for containment and cleaning up",
                Fields =
                [
                    new Sec01SubsectionField
                    {
                        Label = "For containment:",
                        Description =
                            "Set up barriers and cover the sewage system to prevent large-scale spread."
                    },
                    new Sec01SubsectionField
                    {
                        Label = "For cleaning up:",
                        Description =
                            "Immediately absorb with suitable liquid-binding, non-flammable materials (e.g. sand, silica gel, universal binder). Collect bound material in suitable, sealable containers and dispose of in accordance with applicable regulations."
                    }
                ]
            },
            new()
            {
                Heading = "6.4 Reference to other sections",
                Description =
                    "See Section 7 for safe handling, Section 8 for personal protective equipment, and Section 13 for disposal."
            }
        ];

        public static string FromLegacy(Chemical c)
        {
            var items = DefaultTemplate();

            var sec61 = items[0].Fields;
            if (sec61 != null)
            {
                SetField(sec61, "non-emergency", c.SpillNonEmergencyPersonnel);
                SetField(sec61, "emergency personnel", c.SpillEmergencyResponders);
            }

            if (!string.IsNullOrWhiteSpace(c.EnvironmentalPrecautionsSpill))
                items[1].Description = c.EnvironmentalPrecautionsSpill.Trim();

            var sec63 = items[2].Fields;
            if (sec63 != null)
            {
                SetField(sec63, "containment", c.LargeSpillMethods);
                SetField(sec63, "cleaning", c.SmallSpillMethods);
            }

            return Serialize(items);
        }

        private static void SetField(List<Sec01SubsectionField> fields, string labelPrefix, string? value)
        {
            if (string.IsNullOrWhiteSpace(value)) return;
            var field = fields.FirstOrDefault(f =>
                f.Label.TrimStart().StartsWith(labelPrefix, StringComparison.OrdinalIgnoreCase) ||
                f.Label.Contains(labelPrefix, StringComparison.OrdinalIgnoreCase));
            if (field != null)
                field.Description = value.Trim();
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

            var sec61 = rows.FirstOrDefault(r =>
                r.Heading.TrimStart().StartsWith("6.1", StringComparison.OrdinalIgnoreCase));

            target.SpillNonEmergencyPersonnel =
                FieldDesc(sec61, "non-emergency") ?? target.SpillNonEmergencyPersonnel;
            target.SpillEmergencyResponders =
                FieldDesc(sec61, "emergency") ?? target.SpillEmergencyResponders;

            var sec62 = rows.FirstOrDefault(r =>
                r.Heading.TrimStart().StartsWith("6.2", StringComparison.OrdinalIgnoreCase));
            if (!string.IsNullOrWhiteSpace(sec62?.Description))
                target.EnvironmentalPrecautionsSpill = sec62.Description.Trim();

            var sec63 = rows.FirstOrDefault(r =>
                r.Heading.TrimStart().StartsWith("6.3", StringComparison.OrdinalIgnoreCase));

            var containment = FieldDesc(sec63, "containment");
            var cleaning = FieldDesc(sec63, "cleaning");
            if (!string.IsNullOrWhiteSpace(containment))
                target.LargeSpillMethods = containment;
            if (!string.IsNullOrWhiteSpace(cleaning))
                target.SmallSpillMethods = cleaning;
        }
    }
}
