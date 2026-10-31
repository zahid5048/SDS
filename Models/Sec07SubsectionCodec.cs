using System.Text.Json;
using System.Text.Json.Serialization;

namespace ChemicalSDS.Models
{
    public static class Sec07SubsectionCodec
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
            string.IsNullOrWhiteSpace(c.Sec07Subsections) ? FromLegacy(c) : c.Sec07Subsections!;

        public static List<Sec01SubsectionItem> DefaultTemplate() =>
        [
            new()
            {
                Heading = "7.1 Precautions for safe handling",
                Description =
                    "Avoid contact with skin and eyes. Do not smoke. Avoid open flames and other sources of ignition. Do not inhale vapours. Ensure good ventilation during use.",
                Fields =
                [
                    new Sec01SubsectionField
                    {
                        Label = "Protective measures:",
                        Description =
                            "Ensure effective ventilation. Keep away from ignition sources and hot surfaces. Take precautionary measures against static discharge."
                    },
                    new Sec01SubsectionField
                    {
                        Label = "Occupational hygiene:",
                        Description =
                            "Do not eat, drink or smoke when using this product. Wash hands thoroughly after handling. Wash contaminated clothing before reuse."
                    }
                ]
            },
            new()
            {
                Heading = "7.2 Conditions for safe storage, including any incompatibilities",
                Fields =
                [
                    new Sec01SubsectionField
                    {
                        Label = "Store rooms and receptacles:",
                        Description =
                            "Keep container tightly closed. Store at 10–30°C. Protect from light."
                    },
                    new Sec01SubsectionField
                    {
                        Label = "Storage class:",
                        Description =
                            "Storage and handling in accordance with AS 1940 — The storage and handling of flammable and combustible liquids; storage class based on the flash point of 80°C (Pensky-Martens closed cup). Segregate per the ADG Code table for UN 3082, Class 9, Packing Group III. Do not store with explosives, oxidising substances, organic peroxides, or highly toxic or infectious materials."
                    },
                    new Sec01SubsectionField
                    {
                        Label = "Further information about storage conditions:",
                        Description =
                            "Product may discolour on prolonged storage. Crystallisation may occur at low temperatures."
                    }
                ]
            },
            new()
            {
                Heading = "7.3 Specific end use(s)",
                Description = "Perfuming of consumer products."
            }
        ];

        public static string FromLegacy(Chemical c)
        {
            var items = DefaultTemplate();

            if (!string.IsNullOrWhiteSpace(c.HandlingPrecautions))
                items[0].Description = c.HandlingPrecautions.Trim();
            SetField(items[0].Fields, "Occupational hygiene", c.OccupationalHygiene);

            if (!string.IsNullOrWhiteSpace(c.SafeStorageConditions))
                SetField(items[1].Fields, "Store rooms", c.SafeStorageConditions);
            if (!string.IsNullOrWhiteSpace(c.IncompatibleMaterials))
                SetField(items[1].Fields, "Storage class", c.IncompatibleMaterials);

            if (!string.IsNullOrWhiteSpace(c.ProductUse))
                items[2].Description = c.ProductUse.Trim();

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

            var sec71 = rows.FirstOrDefault(r =>
                r.Heading.TrimStart().StartsWith("7.1", StringComparison.OrdinalIgnoreCase));
            if (!string.IsNullOrWhiteSpace(sec71?.Description))
                target.HandlingPrecautions = sec71.Description.Trim();
            target.OccupationalHygiene =
                FieldDesc(sec71, "Occupational hygiene") ?? target.OccupationalHygiene;

            var sec72 = rows.FirstOrDefault(r =>
                r.Heading.TrimStart().StartsWith("7.2", StringComparison.OrdinalIgnoreCase));
            target.SafeStorageConditions =
                FieldDesc(sec72, "Store rooms") ?? target.SafeStorageConditions;
            target.IncompatibleMaterials =
                FieldDesc(sec72, "Storage class") ?? target.IncompatibleMaterials;

            var sec73 = rows.FirstOrDefault(r =>
                r.Heading.TrimStart().StartsWith("7.3", StringComparison.OrdinalIgnoreCase));
            if (!string.IsNullOrWhiteSpace(sec73?.Description))
                target.ProductUse = sec73.Description.Trim();
        }
    }
}
