using Microsoft.EntityFrameworkCore;
using ChemicalSDS.Models;

namespace ChemicalSDS.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Chemical> Chemicals { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(u => u.Username).IsUnique();
                entity.HasIndex(u => u.Email).IsUnique();
            });

            modelBuilder.Entity<Chemical>(entity =>
            {
                entity.Property(c => c.Quantity).HasPrecision(18, 2);
            });

            // Ethanol - Complete SDS Data (from your document)
            modelBuilder.Entity<Chemical>().HasData(new Chemical
            {
                Id = 1,

                // SECTION 1: IDENTIFICATION
                ProductIdentifier = "Ethanol",
                ChemicalName = "ethanol",
                OtherMeansOfId = "ethyl alcohol; ALCOHOL; Ethyl alcohol (Ethanol); EtOH; Grain alcohol; Cologne spirit; Denatured Alcohol; METHYLCARBINOL",
                ProductType = "Liquid.",
                ProductUse = "Synthetic/Analytical chemistry.",
                Synonym = "ethyl alcohol; ALCOHOL; Ethyl alcohol (Ethanol); EtOH",
                SDSNumber = "001114",
                SupplierDetails = "Airgas USA, LLC and its affiliates\n259 North Radnor-Chester Road\nSuite 100\nRadnor, PA 19087-5283\n1-610-687-5253",
                EmergencyPhone = "1-866-734-3438",

                // SECTION 2: HAZARDS IDENTIFICATION
                OSHASstatus = "This material is considered hazardous by the OSHA Hazard Communication Standard (29 CFR 1910.1200).",
                Classification = "FLAMMABLE LIQUIDS - Category 2",
                SignalWord = "Danger",
                HazardStatements = "May form explosive mixtures with air.\nHighly flammable liquid and vapor.",
                PrecautionaryGeneral = "Read label before use. Keep out of reach of children. If medical advice is needed, have product container or label at hand.",
                PrecautionaryPrevention = "Wear protective gloves. Wear eye or face protection. Keep away from heat, sparks, open flames and hot surfaces. - No smoking. Use explosion-proof electrical, ventilating, lighting and all material-handling equipment. Use only non-sparking tools. Take precautionary measures against static discharge. Keep container tightly closed. Use and store only outdoors or in a well ventilated place.",
                PrecautionaryResponse = "IF ON SKIN (or hair): Take off immediately all contaminated clothing. Rinse skin with water or shower.",
                PrecautionaryStorage = "Store in a well-ventilated place. Keep cool.",
                PrecautionaryDisposal = "Dispose of contents and container in accordance with all local, regional, national and international regulations.",

                // SECTION 3: COMPOSITION
                CASNumber = "64-17-5",
                IngredientName = "ethanol",
                Percentage = "100",

                // SECTION 4: FIRST AID
                EyeContactFirstAid = "Immediately flush eyes with plenty of water, occasionally lifting the upper and lower eyelids. Check for and remove any contact lenses. Continue to rinse for at least 10 minutes. Get medical attention if irritation occurs.",
                InhalationFirstAid = "Remove victim to fresh air and keep at rest in a position comfortable for breathing. If not breathing, if breathing is irregular or if respiratory arrest occurs, provide artificial respiration or oxygen by trained personnel. It may be dangerous to the person providing aid to give mouth-to-mouth resuscitation. Get medical attention if adverse health effects persist or are severe. If unconscious, place in recovery position and get medical attention immediately.",
                SkinContactFirstAid = "Flush contaminated skin with plenty of water. Remove contaminated clothing and shoes. Get medical attention if symptoms occur. Wash clothing before reuse. Clean shoes thoroughly before reuse.",
                IngestionFirstAid = "Wash out mouth with water. Remove dentures if any. Remove victim to fresh air and keep at rest in a position comfortable for breathing. If material has been swallowed and the exposed person is conscious, give small quantities of water to drink. Stop if the exposed person feels sick as vomiting may be dangerous. Do not induce vomiting unless directed to do so by medical personnel. If vomiting occurs, the head should be kept low so that vomit does not enter the lungs. Get medical attention if adverse health effects persist or are severe. Never give anything by mouth to an unconscious person.",

                // SECTION 5: FIRE FIGHTING
                SuitableExtinguishingMedia = "Use dry chemical, CO₂, water spray (fog) or foam.",
                UnsuitableExtinguishingMedia = "Do not use water jet.",
                SpecificHazards = "Highly flammable liquid and vapor. Runoff to sewer may create fire or explosion hazard. In a fire or if heated, a pressure increase will occur and the container may burst, with the risk of a subsequent explosion. The vapor/gas is heavier than air and will spread along the ground. Vapors may accumulate in low or confined areas or travel a considerable distance to a source of ignition and flash back.",
                FlashPoint = "Closed cup: 9.7°C (49.5°F)",
                AutoIgnitionTemp = "455°C (851°F)",
                LowerExplosiveLimit = "3.3%",
                UpperExplosiveLimit = "19%",

                // SECTION 6: ACCIDENTAL RELEASE
                SmallSpillMethods = "Stop leak if without risk. Move containers from spill area. Use spark-proof tools and explosion-proof equipment. Dilute with water and mop up if water-soluble. Alternatively, or if water-insoluble, absorb with an inert dry material and place in an appropriate waste disposal container. Dispose of via a licensed waste disposal contractor.",
                LargeSpillMethods = "Stop leak if without risk. Move containers from spill area. Use spark-proof tools and explosion-proof equipment. Approach release from upwind. Prevent entry into sewers, water courses, basements or confined areas. Contain and collect spillage with non-combustible, absorbent material e.g. sand, earth, vermiculite or diatomaceous earth and place in container for disposal according to local regulations. Dispose of via a licensed waste disposal contractor.",

                // SECTION 7: HANDLING AND STORAGE
                SafeStorageConditions = "Store in accordance with local regulations. Store in a segregated and approved area. Store in original container protected from direct sunlight in a dry, cool and well-ventilated area, away from incompatible materials and food and drink. Eliminate all ignition sources. Separate from oxidizing materials. Keep container tightly closed and sealed until ready for use.",
                IncompatibleMaterials = "Reactive or incompatible with the following materials: oxidizing materials",

                // SECTION 8: EXPOSURE CONTROLS/PERSONAL PROTECTION
                EyeProtection = "Safety eyewear complying with an approved standard should be used. Safety glasses with side shields.",
                HandProtection = "Chemical-resistant, impervious gloves complying with an approved standard should be worn.",
                BodyProtection = "Personal protective equipment for the body. Anti-static protective clothing.",
                RespiratoryProtection = "Based on the hazard and potential for exposure, select a respirator that meets the appropriate standard or certification.",

                // SECTION 9: PHYSICAL AND CHEMICAL PROPERTIES
                PhysicalState = "Liquid.",
                Color = "Colorless. Clear.",
                Odor = "Characteristic.",
                MeltingPoint = "-114°C (-173.2°F)",
                BoilingPoint = "78.29°C (172.9°F)",
                VaporPressure = "5.7 kPa (42.95 mm Hg) [room temperature]",
                VaporDensity = "1.6 (Air = 1)",
                RelativeDensity = "0.8",
                Solubility = "1000 g/l",

                // SECTION 10: STABILITY AND REACTIVITY
                ChemicalStability = "The product is stable.",
                ConditionsToAvoid = "Avoid all possible sources of ignition (spark or flame). Do not pressurize, cut, weld, braze, solder, drill, grind or expose containers to heat or sources of ignition. Do not allow vapor to accumulate in low or confined areas.",
                HazardousDecomposition = "Decomposition products may include the following materials: carbon dioxide, carbon monoxide",
                HazardousPolymerization = "Under normal conditions of storage and use, hazardous polymerization will not occur.",

                // SECTION 11: TOXICOLOGICAL INFORMATION
                AcuteToxicity = "Not available.",
                IrritationCorrosion = "ethanol|Eyes - Mild irritant|Rabbit|-|24 hours 500 milligrams|-\nethanol|Eyes - Moderate irritant|Rabbit|-|0.066666667 minutes 100 milligrams|-\nethanol|Eyes - Moderate irritant|Rabbit|-|100 microliters|-\nethanol|Eyes - Severe irritant|Rabbit|-|500 milligrams|-\nethanol|Skin - Mild irritant|Rabbit|-|400 milligrams|-\nethanol|Skin - Moderate irritant|Rabbit|-|24 hours 20 milligrams|-",
                Carcinogenicity = "Not available.",
                CarcinogenicityClassification = "ethanol|-|1|-",

                // SECTION 12: ECOLOGICAL INFORMATION
                Ecotoxicity = "Acute EC50 17.921 mg/l Marine water Algae - 96 hours. Acute EC50 2000 µg/l Fresh water Daphnia - 48 hours.",
                BioaccumulativePotential = "Not available.",

                // SECTION 13: DISPOSAL CONSIDERATIONS
                DisposalMethods = "The generation of waste should be avoided or minimized wherever possible. Dispose of surplus and non-recyclable products via a licensed waste disposal contractor. Empty containers or liners may retain some product residues. Do not cut, weld or grind used containers unless they have been cleaned thoroughly internally.",

                // SECTION 14: TRANSPORT INFORMATION
                UNNumber = "UN1170",
                UNProperShippingName = "ETHANOL OR ETHYL ALCOHOL OR ETHANOL SOLUTIONS",
                TransportHazardClass = "3",
                PackingGroup = "II",

                // SECTION 15: REGULATORY INFORMATION
                RegTscaCdrExempt = "Not determined",
                RegCleanAirActHaps = "Not listed",
                RegCleanAirActClassI = "Not listed",
                RegCleanAirActClassII = "Not listed",
                RegDeaListI = "Not listed",
                RegDeaListII = "Not listed",
                RegSara302Composition = "No products were found.",
                RegSara304RQ = "Not applicable.",
                RegSara311312Classification = "Refer to Section 2: Hazards Identification of this SDS for classification of substance.",
                RegStateMassachusetts = "This material is listed.",
                RegStateNewYork = "This material is not listed.",
                RegStateNewJersey = "This material is listed.",
                RegStatePennsylvania = "This material is listed.",
                RegChemWeaponConvention = "Not listed.",
                RegMontrealProtocol = "Not listed.",
                RegStockholmConvention = "Not listed.",
                RegRotterdamConvention = "Not listed.",
                RegUneceAarhus = "Not listed.",
                RegInventoryAustralia = "This material is listed or exempted.",
                RegInventoryCanada = "This material is listed or exempted.",
                RegInventoryChina = "This material is listed or exempted.",
                RegInventoryEurope = "This material is listed or exempted.",
                RegInventoryJapanEncs = "This material is listed or exempted.",
                RegInventoryJapanIshl = "This material is listed or exempted.",
                RegInventoryMalaysia = "This material is listed or exempted.",
                RegInventoryNewZealand = "This material is listed or exempted.",
                RegInventoryPhilippines = "This material is listed or exempted.",
                RegInventoryKorea = "This material is listed or exempted.",
                RegInventoryTaiwan = "This material is listed or exempted.",
                RegInventoryThailand = "Not determined",
                RegInventoryTurkey = "This material is listed or exempted.",
                RegInventoryUnitedStates = "This material is listed or exempted.",
                RegInventoryVietnam = "Not determined",
                USFederalRegulations = "TSCA 8(a) CDR Exempt/Partial exemption: Not determined. Clean Air Act: Not listed. SARA 302/304: Not listed.",
                StateRegulations = "Massachusetts: This material is listed. New Jersey: This material is listed. Pennsylvania: This material is listed.",
                InventoryList = "Australia: Listed. Canada: Listed. China: Listed. Europe: Listed. Japan: Listed. United States: Listed.",

                // SECTION 16: OTHER INFORMATION
                HmisHealth = "1",
                HmisFlammability = "4",
                HmisPhysicalHazards = "0",
                NfpaHealth = "1",
                NfpaFlammability = "4",
                NfpaReactivity = "0",
                ClassificationProcedure = "FLAMMABLE LIQUIDS - Category 2",
                ClassificationJustification = "Expert judgment",
                References = "Not available.",
                HMISRatings = "Health: 1, Flammability: 4, Physical hazards: 0",
                NFPARatings = "Health: 1, Flammability: 4, Instability/Reactivity: 0",
                Version = "1",
                DateOfRevision = "4/8/2019",
                DateOfPrinting = "4/8/2019",
                DateOfPreviousIssue = "No previous validation",
                NoticeToReader = "To the best of our knowledge, the information contained herein is accurate. However, neither the above-named supplier, nor any of its subsidiaries, assumes any liability whatsoever for the accuracy or completeness of the information contained herein. Final determination of suitability of any material is the sole responsibility of the user. All materials may present unknown hazards and should be used with caution. Although certain hazards are described herein, we cannot guarantee that these are the only hazards that exist.",
                Abbreviations = "ATE = Acute Toxicity Estimate\nBCF = Bioconcentration Factor\nGHS = Globally Harmonized System of Classification and Labelling of Chemicals\nIATA = International Air Transport Association\nIBC = Intermediate Bulk Container\nIMDG = International Maritime Dangerous Goods\nLogPow = logarithm of the octanol/water partition coefficient\nMARPOL = International Convention for the Prevention of Pollution From Ships, 1973 as modified by the Protocol of 1978. (\"Marpol\" = marine pollution)",

                // Inventory
                Quantity = 5.5m,
                Unit = "Liters",
                StorageLocation = "Flammable Cabinet A-12",
                CreatedAt = new DateTime(2024, 1, 1),
                LastCompletedSection = 16,
                IsDraft = false,
                SubstanceMixture = "Substance",
                ProductCode = "001114",
                HandlingPrecautions = "Wear protective gloves. Use only with adequate ventilation.",
                ExposureControls = "Use only with adequate ventilation. Use explosion-proof ventilation equipment.",
                HazardsNotOtherwiseClassified = "None known."
            });
        }
    }
}