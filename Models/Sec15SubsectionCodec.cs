using System.Text.Json;
using System.Text.Json.Serialization;

namespace ChemicalSDS.Models
{
    public static class Sec15SubsectionCodec
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
            string.IsNullOrWhiteSpace(c.Sec15Subsections) ? "[]" : c.Sec15Subsections!;

        public static List<Sec01SubsectionItem> DefaultTemplate() =>
        [
            new()
            {
                Heading = "15.1 Work Health and Safety Regulations 2012 (and equivalent state and territory WHS Regulations)",
                Description =
                    "Classified and labelled in accordance with the GHS as adopted under Schedule 6 of the WHS Regulations.\n\n" +
                    "Ingredients disclosed in accordance with Schedule 8 of the WHS Regulations — see Section 3.2."
            },
            new()
            {
                Heading = "15.2 Schedule 10 — prohibited carcinogens, restricted carcinogens and restricted hazardous chemicals",
                Description =
                    "Checked under regulations 340 and 380–384 and Schedule 10 of the WHS Regulations 2012. None of the ingredients in this product appear on Table 10.1 (prohibited carcinogens) or Table 10.2 (restricted carcinogens). Table 10.3 (restricted hazardous chemicals) covers metals and compounds used in abrasive blasting and spray painting and has no application to this product.\n\n" +
                    "Any Carcinogenicity Category 2 finding for an ingredient (see Section 3.2) is a labelling and classification matter under Schedule 6, not a supply prohibition or restricted-use matter under Schedule 10 unless the ingredient is listed on Table 10.1 or 10.2."
            },
            new()
            {
                Heading = "15.3 Australian Industrial Chemicals Introduction Scheme (AICIS)",
                Description =
                    "All ingredients in this product are listed on the AICIS Inventory.\n\n" +
                    "Document any ongoing specific information requirements (SIR), 28-day notification duties, or concentration limits for individual CAS numbers here (refer to Section 3.2).\n\n" +
                    "Action for downstream customers: concentration limits and notification duties for individual ingredients apply to finished consumer products into which this material is formulated, not necessarily to this product as supplied. Whoever formulates this material into a finished product needs to track relevant thresholds and hold any applicable AICIS notification duty.\n\n" +
                    "None of the ingredients in this product carry a Defined Scope of Assessment restriction, additional Conditions of Introduction and Use, or Prescribed Information entry. AICIS registration is also required of the Australian importer/manufacturer as an entity."
            },
            new()
            {
                Heading = "15.4 Poisons Standard (SUSMP)",
                Description = "Scheduling status: not determined."
            },
            new()
            {
                Heading = "15.5 Major hazard facility notification",
                Description =
                    "Major hazard facility status under WHS Regulations Chapter 9 depends on quantities held on site, which is outside the scope of this document — assess separately if relevant."
            }
        ];

        

        

        
    }
}
