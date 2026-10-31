using System.Text.Json;
using System.Text.Json.Serialization;

namespace ChemicalSDS.Models
{
    public static class Sec05SubsectionCodec
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
            string.IsNullOrWhiteSpace(c.Sec05Subsections) ? FromLegacy(c) : c.Sec05Subsections!;

        public static List<Sec01SubsectionItem> DefaultTemplate() =>
        [
            new()
            {
                Heading = "5.1 Extinguishing media",
                Fields =
                [
                    new Sec01SubsectionField
                    {
                        Label = "Suitable extinguishing agents:",
                        Description = "dry extinguishing agent, alcohol-resistant foam, carbon dioxide, water spray."
                    },
                    new Sec01SubsectionField
                    {
                        Label = "Unsuitable extinguishing agents:",
                        Description = "a direct water jet can spread the fire."
                    }
                ]
            },
            new()
            {
                Heading = "5.2 Special hazard arising from the substance or mixture",
                Description =
                    "Do not allow wastewater from firefighting to enter sewers, surface waters or groundwater."
            },
            new()
            {
                Heading = "5.3 Advice for fire-fighters",
                Description =
                    "Avoid inhalation of smoke and fumes. In case of insufficient ventilation, use breathing apparatus. Contaminated extinguishing water must not be allowed to enter the sewage system and must be disposed of in accordance with applicable environmental regulations."
            }
        ];

        public static string FromLegacy(Chemical c)
        {
            var items = DefaultTemplate();
            var sec51 = items[0].Fields;
            if (sec51 != null)
            {
                SetField(sec51, "Suitable", c.SuitableExtinguishingMedia);
                SetField(sec51, "Unsuitable", c.UnsuitableExtinguishingMedia);
            }

            var sec52Parts = new List<string>();
            if (!string.IsNullOrWhiteSpace(c.SpecificHazards))
                sec52Parts.Add(c.SpecificHazards.Trim());
            if (!string.IsNullOrWhiteSpace(c.HazardousDecomposition))
                sec52Parts.Add("Hazardous thermal decomposition products: " + c.HazardousDecomposition.Trim());
            if (sec52Parts.Count > 0)
                items[1].Description = string.Join("\n\n", sec52Parts);

            var sec53Parts = new List<string>();
            if (!string.IsNullOrWhiteSpace(c.FirefighterProtectiveActions))
                sec53Parts.Add(c.FirefighterProtectiveActions.Trim());
            if (!string.IsNullOrWhiteSpace(c.FirefighterProtectiveEquipment))
                sec53Parts.Add(c.FirefighterProtectiveEquipment.Trim());
            if (sec53Parts.Count > 0)
                items[2].Description = string.Join("\n\n", sec53Parts);

            var extra = new List<string>();
            if (!string.IsNullOrWhiteSpace(c.FlashPoint))
                extra.Add("Flash point: " + c.FlashPoint.Trim());
            if (!string.IsNullOrWhiteSpace(c.AutoIgnitionTemp))
                extra.Add("Auto-ignition temperature: " + c.AutoIgnitionTemp.Trim());
            if (!string.IsNullOrWhiteSpace(c.LowerExplosiveLimit))
                extra.Add("Lower explosive limit: " + c.LowerExplosiveLimit.Trim());
            if (!string.IsNullOrWhiteSpace(c.UpperExplosiveLimit))
                extra.Add("Upper explosive limit: " + c.UpperExplosiveLimit.Trim());
            if (extra.Count > 0)
            {
                items.Add(new Sec01SubsectionItem
                {
                    Heading = "Additional fire-related data",
                    Description = string.Join("\n", extra)
                });
            }

            return Serialize(items);
        }

        private static void SetField(List<Sec01SubsectionField> fields, string labelPrefix, string? value)
        {
            if (string.IsNullOrWhiteSpace(value)) return;
            var field = fields.FirstOrDefault(f =>
                f.Label.TrimStart().StartsWith(labelPrefix, StringComparison.OrdinalIgnoreCase));
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
                    x.Label.TrimStart().StartsWith(labelPrefix, StringComparison.OrdinalIgnoreCase));
                return string.IsNullOrWhiteSpace(f?.Description) ? null : f.Description.Trim();
            }

            var sec51 = rows.FirstOrDefault(r =>
                r.Heading.TrimStart().StartsWith("5.1", StringComparison.OrdinalIgnoreCase));

            target.SuitableExtinguishingMedia =
                FieldDesc(sec51, "Suitable") ?? target.SuitableExtinguishingMedia;
            target.UnsuitableExtinguishingMedia =
                FieldDesc(sec51, "Unsuitable") ?? target.UnsuitableExtinguishingMedia;

            var sec52 = rows.FirstOrDefault(r =>
                r.Heading.TrimStart().StartsWith("5.2", StringComparison.OrdinalIgnoreCase));
            if (!string.IsNullOrWhiteSpace(sec52?.Description))
                target.SpecificHazards = sec52.Description.Trim();

            var sec53 = rows.FirstOrDefault(r =>
                r.Heading.TrimStart().StartsWith("5.3", StringComparison.OrdinalIgnoreCase));
            if (!string.IsNullOrWhiteSpace(sec53?.Description))
                target.FirefighterProtectiveActions = sec53.Description.Trim();
        }
    }
}
