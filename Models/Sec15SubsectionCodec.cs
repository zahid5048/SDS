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
            string.IsNullOrWhiteSpace(c.Sec15Subsections) ? FromLegacy(c) : c.Sec15Subsections!;

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

        public static string FromLegacy(Chemical c)
        {
            var items = DefaultTemplate();

            if (!string.IsNullOrWhiteSpace(c.RegInventoryAustralia))
            {
                var note = c.RegInventoryAustralia.Trim();
                if (!note.Contains("AICIS", StringComparison.OrdinalIgnoreCase))
                    note = "AICIS inventory: " + note;
                items[2].Description = note + "\n\n" + items[2].Description;
            }

            var legacy = BuildLegacyUsInventoryBlock(c);
            if (!string.IsNullOrWhiteSpace(legacy))
            {
                items.Add(new Sec01SubsectionItem
                {
                    Heading = "Legacy regulatory information (previous US / multi-country format)",
                    Description = legacy
                });
            }

            if (!string.IsNullOrWhiteSpace(c.USFederalRegulations))
            {
                items.Add(new Sec01SubsectionItem
                {
                    Heading = "U.S. Federal regulations (legacy summary)",
                    Description = c.USFederalRegulations.Trim()
                });
            }

            if (!string.IsNullOrWhiteSpace(c.InternationalRegulations))
            {
                items.Add(new Sec01SubsectionItem
                {
                    Heading = "International regulations (legacy summary)",
                    Description = c.InternationalRegulations.Trim()
                });
            }

            if (!string.IsNullOrWhiteSpace(c.InventoryList))
            {
                items.Add(new Sec01SubsectionItem
                {
                    Heading = "Inventory list (legacy summary)",
                    Description = c.InventoryList.Trim()
                });
            }

            return Serialize(items);
        }

        private static string? BuildLegacyUsInventoryBlock(Chemical c)
        {
            var lines = new List<string>();
            void Add(string label, string? value)
            {
                if (!string.IsNullOrWhiteSpace(value))
                    lines.Add($"{label}: {value.Trim()}");
            }

            Add("TSCA 8(a) CDR Exempt/Partial exemption", c.RegTscaCdrExempt);
            Add("Clean Air Act Section 112 (b) HAPs", c.RegCleanAirActHaps);
            Add("Clean Air Act Section 602 Class I", c.RegCleanAirActClassI);
            Add("Clean Air Act Section 602 Class II", c.RegCleanAirActClassII);
            Add("DEA List I", c.RegDeaListI);
            Add("DEA List II", c.RegDeaListII);
            Add("SARA 302/304 — Composition", c.RegSara302Composition ?? c.SARA302304);
            Add("SARA 304 RQ", c.RegSara304RQ);
            Add("SARA 311/312 — Classification", c.RegSara311312Classification);
            Add("Massachusetts", c.RegStateMassachusetts);
            Add("New York", c.RegStateNewYork);
            Add("New Jersey", c.RegStateNewJersey);
            Add("Pennsylvania", c.RegStatePennsylvania);
            Add("Chemical Weapon Convention", c.RegChemWeaponConvention);
            Add("Montreal Protocol", c.RegMontrealProtocol);
            Add("Stockholm Convention", c.RegStockholmConvention);
            Add("Rotterdam Convention (PIC)", c.RegRotterdamConvention);
            Add("UNECE Aarhus Protocol", c.RegUneceAarhus);
            Add("Inventory — Canada", c.RegInventoryCanada);
            Add("Inventory — China", c.RegInventoryChina);
            Add("Inventory — Europe", c.RegInventoryEurope);
            Add("Inventory — Japan (ENCS)", c.RegInventoryJapanEncs);
            Add("Inventory — Japan (ISHL)", c.RegInventoryJapanIshl);
            Add("Inventory — Malaysia", c.RegInventoryMalaysia);
            Add("Inventory — New Zealand", c.RegInventoryNewZealand);
            Add("Inventory — Philippines", c.RegInventoryPhilippines);
            Add("Inventory — Republic of Korea", c.RegInventoryKorea);
            Add("Inventory — Taiwan", c.RegInventoryTaiwan);
            Add("Inventory — Thailand", c.RegInventoryThailand);
            Add("Inventory — Turkey", c.RegInventoryTurkey);
            Add("Inventory — United States", c.RegInventoryUnitedStates);
            Add("Inventory — Viet Nam", c.RegInventoryVietnam);

            return lines.Count == 0 ? null : string.Join("\n", lines);
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

            var aicis = Desc(rows, "15.3");
            if (!string.IsNullOrWhiteSpace(aicis))
            {
                var firstLine = aicis.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                    .FirstOrDefault();
                if (!string.IsNullOrWhiteSpace(firstLine))
                    target.RegInventoryAustralia = firstLine.Length > 500 ? firstLine[..500] : firstLine;
            }

            var susmp = Desc(rows, "15.4");
            if (!string.IsNullOrWhiteSpace(susmp))
                target.StateRegulations = susmp;
        }
    }
}
