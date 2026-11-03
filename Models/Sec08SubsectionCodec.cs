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
            string.IsNullOrWhiteSpace(c.Sec08Subsections) ? "[]" : c.Sec08Subsections!;

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

        
    }
}
