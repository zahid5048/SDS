using System.Text.Json;
using System.Text.Json.Serialization;

namespace ChemicalSDS.Models
{
    public static class Sec09SubsectionCodec
    {
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };

        private const string NotDetermined = "Not determined";

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
            string.IsNullOrWhiteSpace(c.Sec09Subsections) ? FromLegacy(c) : c.Sec09Subsections!;

        public static List<Sec01SubsectionItem> DefaultTemplate() =>
        [
            new()
            {
                Heading = "9.1 Information on basic physical and chemical properties",
                Fields = DefaultPropertyFields()
            },
            new()
            {
                Heading = "9.2 Other information",
                Fields =
                [
                    new Sec01SubsectionField { Label = "Solvent content:", Description = "49.9%" },
                    new Sec01SubsectionField { Label = "VOC:", Description = "3.28%" }
                ]
            }
        ];

        private static List<Sec01SubsectionField> DefaultPropertyFields() =>
        [
            new() { Label = "State of aggregation:", Description = "Liquid" },
            new() { Label = "Colour:", Description = "Light yellow" },
            new() { Label = "Odour:", Description = "Fresh – woody – marine" },
            new() { Label = "Melting point / freezing point:", Description = NotDetermined },
            new() { Label = "Boiling point / boiling range:", Description = NotDetermined },
            new() { Label = "Flammability:", Description = NotDetermined },
            new() { Label = "Lower and upper explosion limits:", Description = NotDetermined },
            new() { Label = "Flash point:", Description = "80.0°C (method: Pensky-Martens closed cup)" },
            new() { Label = "Ignition temperature:", Description = NotDetermined },
            new() { Label = "Decomposition temperature:", Description = NotDetermined },
            new() { Label = "pH value:", Description = "Non-aqueous product" },
            new() { Label = "Kinematic viscosity:", Description = NotDetermined },
            new() { Label = "Solubility:", Description = NotDetermined },
            new() { Label = "Partition coefficient n-octanol/water (log value):", Description = NotDetermined },
            new() { Label = "Vapour pressure:", Description = NotDetermined },
            new() { Label = "Relative density:", Description = "0.9807" },
            new() { Label = "Relative vapour density:", Description = NotDetermined },
            new() { Label = "Particulate properties:", Description = NotDetermined }
        ];

        public static string FromLegacy(Chemical c)
        {
            var items = DefaultTemplate();
            var f91 = items[0].Fields!;

            SetField(f91, "State of aggregation", c.PhysicalState);
            SetField(f91, "Colour", c.Color);
            SetField(f91, "Odour", c.Odor);
            SetField(f91, "Melting point", c.MeltingPoint);
            SetField(f91, "Boiling point", c.BoilingPoint);
            SetField(f91, "Flammability", c.Flammability);
            var limits = JoinNonEmpty(c.LowerExplosiveLimit, c.UpperExplosiveLimit, " / ");
            SetField(f91, "explosion limits", limits);
            SetField(f91, "Flash point", c.FlashPoint);
            SetField(f91, "Ignition temperature", c.AutoIgnitionTemp);
            SetField(f91, "Decomposition temperature", c.DecompositionTemperature);
            SetField(f91, "pH value", c.pH);
            SetField(f91, "Kinematic viscosity", c.Viscosity);
            var sol = JoinNonEmpty(c.Solubility, c.SolubilityInWater, "; ");
            SetField(f91, "Solubility", sol);
            SetField(f91, "Partition coefficient", c.PartitionCoefficient);
            SetField(f91, "Vapour pressure", c.VaporPressure);
            SetField(f91, "Relative density", c.RelativeDensity);
            SetField(f91, "Relative vapour density", c.VaporDensity);
            var extraParts = new[]
            {
                LabelValue("Odor threshold", c.OdorThreshold),
                LabelValue("Evaporation rate", c.EvaporationRate),
                LabelValue("Critical temperature", c.CriticalTemperature),
                LabelValue("Flow time (ISO 2431)", c.FlowTimeIso2431),
                LabelValue("Molecular weight", c.MolecularWeight)
            }.Where(s => !string.IsNullOrWhiteSpace(s));
            var extra = string.Join("\n", extraParts);
            if (string.IsNullOrWhiteSpace(extra)) extra = null;
            if (!string.IsNullOrWhiteSpace(extra))
            {
                var part = FieldDesc(f91, "Particulate");
                SetField(f91, "Particulate", string.IsNullOrWhiteSpace(part) ? extra : part + "\n" + extra);
            }

            var f92 = items[1].Fields!;
            SetField(f92, "Solvent content", c.Sec09SolventContent);
            SetField(f92, "VOC", c.Sec09Voc);

            return Serialize(items);
        }

        private static string? JoinNonEmpty(string? a, string? b, string sep)
        {
            var parts = new[] { a?.Trim(), b?.Trim() }.Where(s => !string.IsNullOrWhiteSpace(s)).ToList();
            return parts.Count == 0 ? null : string.Join(sep, parts);
        }

        private static string? LabelValue(string label, string? value) =>
            string.IsNullOrWhiteSpace(value) ? null : $"{label}: {value.Trim()}";

        private static void SetField(List<Sec01SubsectionField> fields, string labelPart, string? value)
        {
            if (string.IsNullOrWhiteSpace(value)) return;
            var field = fields.FirstOrDefault(f =>
                f.Label.Contains(labelPart, StringComparison.OrdinalIgnoreCase));
            if (field != null)
                field.Description = value.Trim();
        }

        private static string? FieldDesc(List<Sec01SubsectionField> fields, string labelPart)
        {
            var f = fields.FirstOrDefault(x =>
                x.Label.Contains(labelPart, StringComparison.OrdinalIgnoreCase));
            return string.IsNullOrWhiteSpace(f?.Description) ? null : f.Description.Trim();
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

            var sec91 = rows.FirstOrDefault(r =>
                r.Heading.TrimStart().StartsWith("9.1", StringComparison.OrdinalIgnoreCase));

            target.PhysicalState = GetField(sec91, "State of aggregation") ?? target.PhysicalState;
            target.Color = GetField(sec91, "Colour") ?? target.Color;
            target.Odor = GetField(sec91, "Odour") ?? target.Odor;
            target.MeltingPoint = GetField(sec91, "Melting point") ?? target.MeltingPoint;
            target.BoilingPoint = GetField(sec91, "Boiling point") ?? target.BoilingPoint;
            target.Flammability = GetField(sec91, "Flammability") ?? target.Flammability;
            target.FlashPoint = GetField(sec91, "Flash point") ?? target.FlashPoint;
            target.AutoIgnitionTemp = GetField(sec91, "Ignition temperature") ?? target.AutoIgnitionTemp;
            target.DecompositionTemperature =
                GetField(sec91, "Decomposition temperature") ?? target.DecompositionTemperature;
            target.pH = GetField(sec91, "pH value") ?? target.pH;
            target.Viscosity = GetField(sec91, "Kinematic viscosity") ?? target.Viscosity;
            target.Solubility = GetField(sec91, "Solubility") ?? target.Solubility;
            target.PartitionCoefficient =
                GetField(sec91, "Partition coefficient") ?? target.PartitionCoefficient;
            target.VaporPressure = GetField(sec91, "Vapour pressure") ?? target.VaporPressure;
            target.RelativeDensity = GetField(sec91, "Relative density") ?? target.RelativeDensity;
            target.VaporDensity = GetField(sec91, "Relative vapour density") ?? target.VaporDensity;

            var sec92 = rows.FirstOrDefault(r =>
                r.Heading.TrimStart().StartsWith("9.2", StringComparison.OrdinalIgnoreCase));
            target.Sec09SolventContent = GetField(sec92, "Solvent content") ?? target.Sec09SolventContent;
            target.Sec09Voc = GetField(sec92, "VOC") ?? target.Sec09Voc;
        }
    }
}
