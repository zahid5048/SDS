using System.Text.Json;
using System.Text.Json.Serialization;

namespace ChemicalSDS.Models
{
    public static class Sec14SubsectionCodec
    {
        private const string AdgLabel = "ADR / IMDG / IATA / ADG:";

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
            string.IsNullOrWhiteSpace(c.Sec14Subsections) ? FromLegacy(c) : c.Sec14Subsections!;

        public static List<Sec01SubsectionItem> DefaultTemplate() =>
        [
            new()
            {
                Heading = "14.1 UN number or ID number",
                Description = $"{AdgLabel}\nUN 3082"
            },
            new()
            {
                Heading = "14.2 UN proper shipping name",
                Description =
                    "Environmentally hazardous substance, liquid, N.O.S. (1-(2,3,8,8-Tetramethyl-1,3,4,5,6,7-hexahydronaphthalen-2-yl) ethanone, Mixture)"
            },
            new()
            {
                Heading = "14.3 Transport hazard class(es)",
                Description = $"{AdgLabel}\n9"
            },
            new()
            {
                Heading = "14.4 Packing group",
                Description = $"{AdgLabel}\nIII"
            },
            new()
            {
                Heading = "14.5 Environmental hazards",
                Description =
                    "Marine pollutant / environmentally hazardous substance markings apply (ADR and IATA). EMS code: F-A, S-F."
            },
            new()
            {
                Heading = "14.6 Special precautions for the user",
                Description =
                    "The transport classifications provided are based on the properties of the unpackaged material according to this safety data sheet. Refer to Sections 2, 7 and 8."
            },
            new()
            {
                Heading = "14.7 Carriage of bulk cargo by sea in accordance with IMO instruments",
                Description = "Not applicable."
            }
        ];

        public static string FromLegacy(Chemical c)
        {
            var items = DefaultTemplate();

            if (!string.IsNullOrWhiteSpace(c.UNNumber))
                items[0].Description = $"{AdgLabel}\n{FormatUn(c.UNNumber)}";
            if (!string.IsNullOrWhiteSpace(c.UNProperShippingName))
                items[1].Description = c.UNProperShippingName.Trim();
            if (!string.IsNullOrWhiteSpace(c.TransportHazardClass))
                items[2].Description = $"{AdgLabel}\n{c.TransportHazardClass.Trim()}";
            if (!string.IsNullOrWhiteSpace(c.PackingGroup))
                items[3].Description = $"{AdgLabel}\n{c.PackingGroup.Trim()}";
            if (!string.IsNullOrWhiteSpace(c.EnvironmentalHazardsTransport))
                items[4].Description = c.EnvironmentalHazardsTransport.Trim();
            if (!string.IsNullOrWhiteSpace(c.TransportSpecialPrecautions))
                items[5].Description = c.TransportSpecialPrecautions.Trim();
            if (!string.IsNullOrWhiteSpace(c.TransportBulkMarpol))
                items[6].Description = c.TransportBulkMarpol.Trim();

            var extra = new List<string>();
            if (!string.IsNullOrWhiteSpace(c.TransportDotClassification))
                extra.Add("DOT: " + c.TransportDotClassification.Trim());
            if (!string.IsNullOrWhiteSpace(c.TransportTdgClassification))
                extra.Add("TDG: " + c.TransportTdgClassification.Trim());
            if (!string.IsNullOrWhiteSpace(c.TransportIataClassification))
                extra.Add("IATA: " + c.TransportIataClassification.Trim());
            if (extra.Count > 0)
            {
                items.Add(new Sec01SubsectionItem
                {
                    Heading = "Additional transport information (legacy)",
                    Description = string.Join("\n\n", extra)
                });
            }

            return Serialize(items);
        }

        private static string FormatUn(string un) =>
            un.TrimStart().StartsWith("UN", StringComparison.OrdinalIgnoreCase) ? un.Trim() : "UN " + un.Trim();

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

            static string? ValueAfterAdgLabel(string? block)
            {
                if (string.IsNullOrWhiteSpace(block)) return null;
                var lines = block.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
                if (lines.Length >= 2)
                    return lines[^1];
                if (lines.Length == 1 && !lines[0].StartsWith("ADR", StringComparison.OrdinalIgnoreCase))
                    return lines[0];
                return null;
            }

            var d141 = Desc(rows, "14.1");
            var un = ValueAfterAdgLabel(d141);
            if (!string.IsNullOrWhiteSpace(un))
                target.UNNumber = un;

            target.UNProperShippingName = Desc(rows, "14.2") ?? target.UNProperShippingName;

            var cls = ValueAfterAdgLabel(Desc(rows, "14.3"));
            if (!string.IsNullOrWhiteSpace(cls))
                target.TransportHazardClass = cls;

            var pg = ValueAfterAdgLabel(Desc(rows, "14.4"));
            if (!string.IsNullOrWhiteSpace(pg))
                target.PackingGroup = pg;

            target.EnvironmentalHazardsTransport =
                Desc(rows, "14.5") ?? target.EnvironmentalHazardsTransport;
            target.TransportSpecialPrecautions =
                Desc(rows, "14.6") ?? target.TransportSpecialPrecautions;
            target.TransportBulkMarpol = Desc(rows, "14.7") ?? target.TransportBulkMarpol;
        }
    }
}
