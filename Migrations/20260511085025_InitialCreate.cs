using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChemicalSDS.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Chemicals",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductIdentifier = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ChemicalName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OtherMeansOfId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProductType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProductUse = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Synonym = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SDSNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SupplierDetails = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmergencyPhone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OSHASstatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Classification = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SignalWord = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HazardStatements = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PrecautionaryGeneral = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PrecautionaryPrevention = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PrecautionaryResponse = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PrecautionaryStorage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PrecautionaryDisposal = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CASNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IngredientName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Percentage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EyeContactFirstAid = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InhalationFirstAid = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SkinContactFirstAid = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IngestionFirstAid = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SuitableExtinguishingMedia = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UnsuitableExtinguishingMedia = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SpecificHazards = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FlashPoint = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AutoIgnitionTemp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LowerExplosiveLimit = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpperExplosiveLimit = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SmallSpillMethods = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LargeSpillMethods = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SafeStorageConditions = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IncompatibleMaterials = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EyeProtection = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HandProtection = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BodyProtection = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RespiratoryProtection = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhysicalState = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Color = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Odor = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MeltingPoint = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BoilingPoint = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VaporPressure = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VaporDensity = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RelativeDensity = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Solubility = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ChemicalStability = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConditionsToAvoid = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HazardousDecomposition = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AcuteToxicity = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IrritationCorrosion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Carcinogenicity = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Ecotoxicity = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BioaccumulativePotential = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DisposalMethods = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UNNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UNProperShippingName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TransportHazardClass = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PackingGroup = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    USFederalRegulations = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StateRegulations = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InventoryList = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HMISRatings = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NFPARatings = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Version = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DateOfRevision = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NoticeToReader = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Quantity = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Unit = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StorageLocation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Chemicals", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Chemicals",
                columns: new[] { "Id", "AcuteToxicity", "AutoIgnitionTemp", "BioaccumulativePotential", "BodyProtection", "BoilingPoint", "CASNumber", "Carcinogenicity", "ChemicalName", "ChemicalStability", "Classification", "Color", "ConditionsToAvoid", "CreatedAt", "DateOfRevision", "DisposalMethods", "Ecotoxicity", "EmergencyPhone", "EyeContactFirstAid", "EyeProtection", "FlashPoint", "HMISRatings", "HandProtection", "HazardStatements", "HazardousDecomposition", "IncompatibleMaterials", "IngestionFirstAid", "IngredientName", "InhalationFirstAid", "InventoryList", "IrritationCorrosion", "LargeSpillMethods", "LowerExplosiveLimit", "MeltingPoint", "NFPARatings", "NoticeToReader", "OSHASstatus", "Odor", "OtherMeansOfId", "PackingGroup", "Percentage", "PhysicalState", "PrecautionaryDisposal", "PrecautionaryGeneral", "PrecautionaryPrevention", "PrecautionaryResponse", "PrecautionaryStorage", "ProductIdentifier", "ProductType", "ProductUse", "Quantity", "RelativeDensity", "RespiratoryProtection", "SDSNumber", "SafeStorageConditions", "SignalWord", "SkinContactFirstAid", "SmallSpillMethods", "Solubility", "SpecificHazards", "StateRegulations", "StorageLocation", "SuitableExtinguishingMedia", "SupplierDetails", "Synonym", "TransportHazardClass", "UNNumber", "UNProperShippingName", "USFederalRegulations", "Unit", "UnsuitableExtinguishingMedia", "UpperExplosiveLimit", "VaporDensity", "VaporPressure", "Version" },
                values: new object[] { 1, "Not available.", "455°C (851°F)", "Not available.", "Personal protective equipment for the body. Anti-static protective clothing.", "78.29°C (172.9°F)", "64-17-5", "Not available.", "ethanol", "The product is stable.", "FLAMMABLE LIQUIDS - Category 2", "Colorless. Clear.", "Avoid all possible sources of ignition (spark or flame). Do not pressurize, cut, weld, braze, solder, drill, grind or expose containers to heat or sources of ignition. Do not allow vapor to accumulate in low or confined areas.", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "4/8/2019", "The generation of waste should be avoided or minimized wherever possible. Dispose of surplus and non-recyclable products via a licensed waste disposal contractor. Empty containers or liners may retain some product residues. Do not cut, weld or grind used containers unless they have been cleaned thoroughly internally.", "Acute EC50 17.921 mg/l Marine water Algae - 96 hours. Acute EC50 2000 µg/l Fresh water Daphnia - 48 hours.", "1-866-734-3438", "Immediately flush eyes with plenty of water, occasionally lifting the upper and lower eyelids. Check for and remove any contact lenses. Continue to rinse for at least 10 minutes. Get medical attention if irritation occurs.", "Safety eyewear complying with an approved standard should be used. Safety glasses with side shields.", "Closed cup: 9.7°C (49.5°F)", "Health: 1, Flammability: 4, Physical hazards: 0", "Chemical-resistant, impervious gloves complying with an approved standard should be worn.", "May form explosive mixtures with air.\nHighly flammable liquid and vapor.", "Decomposition products may include the following materials: carbon dioxide, carbon monoxide", "Reactive or incompatible with the following materials: oxidizing materials", "Wash out mouth with water. Remove dentures if any. Remove victim to fresh air and keep at rest in a position comfortable for breathing. If material has been swallowed and the exposed person is conscious, give small quantities of water to drink. Stop if the exposed person feels sick as vomiting may be dangerous. Do not induce vomiting unless directed to do so by medical personnel. If vomiting occurs, the head should be kept low so that vomit does not enter the lungs. Get medical attention if adverse health effects persist or are severe. Never give anything by mouth to an unconscious person.", "ethanol", "Remove victim to fresh air and keep at rest in a position comfortable for breathing. If not breathing, if breathing is irregular or if respiratory arrest occurs, provide artificial respiration or oxygen by trained personnel. It may be dangerous to the person providing aid to give mouth-to-mouth resuscitation. Get medical attention if adverse health effects persist or are severe. If unconscious, place in recovery position and get medical attention immediately.", "Australia: Listed. Canada: Listed. China: Listed. Europe: Listed. Japan: Listed. United States: Listed.", "Eyes - Mild irritant Rabbit - 500 milligrams. Skin - Mild irritant Rabbit - 400 milligrams.", "Stop leak if without risk. Move containers from spill area. Use spark-proof tools and explosion-proof equipment. Approach release from upwind. Prevent entry into sewers, water courses, basements or confined areas. Contain and collect spillage with non-combustible, absorbent material e.g. sand, earth, vermiculite or diatomaceous earth and place in container for disposal according to local regulations. Dispose of via a licensed waste disposal contractor.", "3.3%", "-114°C (-173.2°F)", "Health: 1, Flammability: 4, Instability/Reactivity: 0", "To the best of our knowledge, the information contained herein is accurate. However, neither the above-named supplier, nor any of its subsidiaries, assumes any liability whatsoever for the accuracy or completeness of the information contained herein.", "This material is considered hazardous by the OSHA Hazard Communication Standard (29 CFR 1910.1200).", "Characteristic.", "ethyl alcohol; ALCOHOL; Ethyl alcohol (Ethanol); EtOH; Grain alcohol; Cologne spirit; Denatured Alcohol; METHYLCARBINOL", "II", "100", "Liquid.", "Dispose of contents and container in accordance with all local, regional, national and international regulations.", "Read label before use. Keep out of reach of children. If medical advice is needed, have product container or label at hand.", "Wear protective gloves. Wear eye or face protection. Keep away from heat, sparks, open flames and hot surfaces. - No smoking. Use explosion-proof electrical, ventilating, lighting and all material-handling equipment. Use only non-sparking tools. Take precautionary measures against static discharge. Keep container tightly closed. Use and store only outdoors or in a well ventilated place.", "IF ON SKIN (or hair): Take off immediately all contaminated clothing. Rinse skin with water or shower.", "Store in a well-ventilated place. Keep cool.", "Ethanol", "Liquid.", "Synthetic/Analytical chemistry.", 5.5m, "0.8", "Based on the hazard and potential for exposure, select a respirator that meets the appropriate standard or certification.", "001114", "Store in accordance with local regulations. Store in a segregated and approved area. Store in original container protected from direct sunlight in a dry, cool and well-ventilated area, away from incompatible materials and food and drink. Eliminate all ignition sources. Separate from oxidizing materials. Keep container tightly closed and sealed until ready for use.", "Danger", "Flush contaminated skin with plenty of water. Remove contaminated clothing and shoes. Get medical attention if symptoms occur. Wash clothing before reuse. Clean shoes thoroughly before reuse.", "Stop leak if without risk. Move containers from spill area. Use spark-proof tools and explosion-proof equipment. Dilute with water and mop up if water-soluble. Alternatively, or if water-insoluble, absorb with an inert dry material and place in an appropriate waste disposal container. Dispose of via a licensed waste disposal contractor.", "1000 g/l", "Highly flammable liquid and vapor. Runoff to sewer may create fire or explosion hazard. In a fire or if heated, a pressure increase will occur and the container may burst, with the risk of a subsequent explosion. The vapor/gas is heavier than air and will spread along the ground. Vapors may accumulate in low or confined areas or travel a considerable distance to a source of ignition and flash back.", "Massachusetts: This material is listed. New Jersey: This material is listed. Pennsylvania: This material is listed.", "Flammable Cabinet A-12", "Use dry chemical, CO₂, water spray (fog) or foam.", "Airgas USA, LLC and its affiliates\n259 North Radnor-Chester Road\nSuite 100\nRadnor, PA 19087-5283\n1-610-687-5253", "ethyl alcohol; ALCOHOL; Ethyl alcohol (Ethanol); EtOH", "3", "UN1170", "ETHANOL OR ETHYL ALCOHOL OR ETHANOL SOLUTIONS", "TSCA 8(a) CDR Exempt/Partial exemption: Not determined. Clean Air Act: Not listed. SARA 302/304: Not listed.", "Liters", "Do not use water jet.", "19%", "1.6 (Air = 1)", "5.7 kPa (42.95 mm Hg) [room temperature]", "1" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Chemicals");
        }
    }
}
