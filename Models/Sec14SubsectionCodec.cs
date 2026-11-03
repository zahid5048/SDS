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
            string.IsNullOrWhiteSpace(c.Sec14Subsections) ? "[]" : c.Sec14Subsections!;

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

        

        private static string FormatUn(string un) =>
            un.TrimStart().StartsWith("UN", StringComparison.OrdinalIgnoreCase) ? un.Trim() : "UN " + un.Trim();

        
    }
}
