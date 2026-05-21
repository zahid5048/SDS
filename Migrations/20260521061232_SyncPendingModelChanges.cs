using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChemicalSDS.Migrations
{
    /// <inheritdoc />
    public partial class SyncPendingModelChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Chemicals",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Abbreviations", "ClassificationJustification", "ClassificationProcedure", "DateOfPrinting", "HmisFlammability", "HmisHealth", "HmisPhysicalHazards", "NfpaFlammability", "NfpaHealth", "NfpaReactivity", "NoticeToReader", "References" },
                values: new object[] { "ATE = Acute Toxicity Estimate\nBCF = Bioconcentration Factor\nGHS = Globally Harmonized System of Classification and Labelling of Chemicals\nIATA = International Air Transport Association\nIBC = Intermediate Bulk Container\nIMDG = International Maritime Dangerous Goods\nLogPow = logarithm of the octanol/water partition coefficient\nMARPOL = International Convention for the Prevention of Pollution From Ships, 1973 as modified by the Protocol of 1978. (\"Marpol\" = marine pollution)", "Expert judgment", "FLAMMABLE LIQUIDS - Category 2", "4/8/2019", "4", "1", "0", "4", "1", "0", "To the best of our knowledge, the information contained herein is accurate. However, neither the above-named supplier, nor any of its subsidiaries, assumes any liability whatsoever for the accuracy or completeness of the information contained herein. Final determination of suitability of any material is the sole responsibility of the user. All materials may present unknown hazards and should be used with caution. Although certain hazards are described herein, we cannot guarantee that these are the only hazards that exist.", "Not available." });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Chemicals",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Abbreviations", "ClassificationJustification", "ClassificationProcedure", "DateOfPrinting", "HmisFlammability", "HmisHealth", "HmisPhysicalHazards", "NfpaFlammability", "NfpaHealth", "NfpaReactivity", "NoticeToReader", "References" },
                values: new object[] { null, null, null, null, null, null, null, null, null, null, "To the best of our knowledge, the information contained herein is accurate. However, neither the above-named supplier, nor any of its subsidiaries, assumes any liability whatsoever for the accuracy or completeness of the information contained herein.", null });
        }
    }
}
