using System.Text.Json;
using System.Text.Json.Serialization;

namespace ChemicalSDS.Models
{
    public static class Sec10SubsectionCodec
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
            string.IsNullOrWhiteSpace(c.Sec10Subsections) ? FromLegacy(c) : c.Sec10Subsections!;

        public static List<Sec01SubsectionItem> DefaultTemplate() =>
        [
            new()
            {
                Heading = "10.1 Reactivity",
                Description = "No reaction known with water."
            },
            new()
            {
                Heading = "10.2 Chemical stability",
                Description = "Good stability at standard temperature."
            },
            new()
            {
                Heading = "10.3 Possibility of hazardous reactions",
                Description = "No decomposition when properly stored and used."
            },
            new()
            {
                Heading = "10.4 Conditions to avoid",
                Description =
                    "Avoid excessive heat above at least 5°C below the flash point for flammable liquids. Do not heat closed containers."
            },
            new()
            {
                Heading = "10.5 Incompatible materials",
                Description =
                    "Do not store or transport with explosives, oxidising substances, organic peroxides, or highly toxic or infectious materials — see Section 7.2."
            },
            new()
            {
                Heading = "10.6 Hazardous decomposition products",
                Description =
                    "Proper use, contact with water, or storage under the recommended conditions should not produce hazardous decomposition products."
            }
        ];

        public static string FromLegacy(Chemical c)
        {
            var items = DefaultTemplate();
            SetDesc(items, "10.1", c.Reactivity);
            SetDesc(items, "10.2", c.ChemicalStability);
            SetDesc(items, "10.3", c.PossibilityOfHazardousReactions);
            SetDesc(items, "10.4", c.ConditionsToAvoid);
            SetDesc(items, "10.5", c.IncompatibleMaterials);
            SetDesc(items, "10.6", c.HazardousDecomposition);

            if (!string.IsNullOrWhiteSpace(c.HazardousPolymerization))
            {
                items.Add(new Sec01SubsectionItem
                {
                    Heading = "10.7 Hazardous polymerization",
                    Description = c.HazardousPolymerization.Trim()
                });
            }

            return Serialize(items);
        }

        private static void SetDesc(List<Sec01SubsectionItem> items, string prefix, string? value)
        {
            if (string.IsNullOrWhiteSpace(value)) return;
            var row = items.FirstOrDefault(r =>
                r.Heading.TrimStart().StartsWith(prefix, StringComparison.OrdinalIgnoreCase));
            if (row != null)
                row.Description = value.Trim();
        }

        public static void SyncLegacyFields(Chemical target, string? json)
        {
            var rows = Parse(json);
            if (rows.Count == 0)
                return;

            static string? Desc(IEnumerable<Sec01SubsectionItem> list, string prefix)
            {
                var row = list.FirstOrDefault(r =>
                    r.Heading.TrimStart().StartsWith(prefix, StringComparison.OrdinalIgnoreCase));
                return string.IsNullOrWhiteSpace(row?.Description) ? null : row.Description.Trim();
            }

            target.Reactivity = Desc(rows, "10.1") ?? target.Reactivity;
            target.ChemicalStability = Desc(rows, "10.2") ?? target.ChemicalStability;
            target.PossibilityOfHazardousReactions =
                Desc(rows, "10.3") ?? target.PossibilityOfHazardousReactions;
            target.ConditionsToAvoid = Desc(rows, "10.4") ?? target.ConditionsToAvoid;
            target.IncompatibleMaterials = Desc(rows, "10.5") ?? target.IncompatibleMaterials;
            target.HazardousDecomposition = Desc(rows, "10.6") ?? target.HazardousDecomposition;
            target.HazardousPolymerization = Desc(rows, "10.7") ?? target.HazardousPolymerization;
        }
    }
}
