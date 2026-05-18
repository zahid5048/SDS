using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChemicalSDS.Migrations
{
    /// <inheritdoc />
    public partial class SdsWizardFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Abbreviations",
                table: "Chemicals",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ChronicHealthEffects",
                table: "Chemicals",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DateOfPreviousIssue",
                table: "Chemicals",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DateOfPrinting",
                table: "Chemicals",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EnvironmentalHazardsTransport",
                table: "Chemicals",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EvaporationRate",
                table: "Chemicals",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExposureControls",
                table: "Chemicals",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Flammability",
                table: "Chemicals",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HandlingPrecautions",
                table: "Chemicals",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HazardsNotOtherwiseClassified",
                table: "Chemicals",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InternationalRegulations",
                table: "Chemicals",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDraft",
                table: "Chemicals",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "LastCompletedSection",
                table: "Chemicals",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "MobilityInSoil",
                table: "Chemicals",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MolecularWeight",
                table: "Chemicals",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Mutagenicity",
                table: "Chemicals",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OccupationalExposureLimits",
                table: "Chemicals",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OccupationalHygiene",
                table: "Chemicals",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OdorThreshold",
                table: "Chemicals",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OtherEcologicalEffects",
                table: "Chemicals",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PersistenceDegradability",
                table: "Chemicals",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PossibilityOfHazardousReactions",
                table: "Chemicals",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProductCode",
                table: "Chemicals",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Reactivity",
                table: "Chemicals",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RoutesOfExposure",
                table: "Chemicals",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SARA302304",
                table: "Chemicals",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SubstanceMixture",
                table: "Chemicals",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Teratogenicity",
                table: "Chemicals",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TransportSpecialPrecautions",
                table: "Chemicals",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "pH",
                table: "Chemicals",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Chemicals",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Abbreviations", "ChronicHealthEffects", "DateOfPreviousIssue", "DateOfPrinting", "EnvironmentalHazardsTransport", "EvaporationRate", "ExposureControls", "Flammability", "HandlingPrecautions", "HazardsNotOtherwiseClassified", "InternationalRegulations", "IsDraft", "LastCompletedSection", "MobilityInSoil", "MolecularWeight", "Mutagenicity", "OccupationalExposureLimits", "OccupationalHygiene", "OdorThreshold", "OtherEcologicalEffects", "PersistenceDegradability", "PossibilityOfHazardousReactions", "ProductCode", "Reactivity", "RoutesOfExposure", "SARA302304", "SubstanceMixture", "Teratogenicity", "TransportSpecialPrecautions", "pH" },
                values: new object[] { null, null, "No previous validation", null, null, null, "Use only with adequate ventilation. Use explosion-proof ventilation equipment.", null, "Wear protective gloves. Use only with adequate ventilation.", "None known.", null, false, 16, null, null, null, null, null, null, null, null, null, "001114", null, null, null, "Substance", null, null, null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Abbreviations",
                table: "Chemicals");

            migrationBuilder.DropColumn(
                name: "ChronicHealthEffects",
                table: "Chemicals");

            migrationBuilder.DropColumn(
                name: "DateOfPreviousIssue",
                table: "Chemicals");

            migrationBuilder.DropColumn(
                name: "DateOfPrinting",
                table: "Chemicals");

            migrationBuilder.DropColumn(
                name: "EnvironmentalHazardsTransport",
                table: "Chemicals");

            migrationBuilder.DropColumn(
                name: "EvaporationRate",
                table: "Chemicals");

            migrationBuilder.DropColumn(
                name: "ExposureControls",
                table: "Chemicals");

            migrationBuilder.DropColumn(
                name: "Flammability",
                table: "Chemicals");

            migrationBuilder.DropColumn(
                name: "HandlingPrecautions",
                table: "Chemicals");

            migrationBuilder.DropColumn(
                name: "HazardsNotOtherwiseClassified",
                table: "Chemicals");

            migrationBuilder.DropColumn(
                name: "InternationalRegulations",
                table: "Chemicals");

            migrationBuilder.DropColumn(
                name: "IsDraft",
                table: "Chemicals");

            migrationBuilder.DropColumn(
                name: "LastCompletedSection",
                table: "Chemicals");

            migrationBuilder.DropColumn(
                name: "MobilityInSoil",
                table: "Chemicals");

            migrationBuilder.DropColumn(
                name: "MolecularWeight",
                table: "Chemicals");

            migrationBuilder.DropColumn(
                name: "Mutagenicity",
                table: "Chemicals");

            migrationBuilder.DropColumn(
                name: "OccupationalExposureLimits",
                table: "Chemicals");

            migrationBuilder.DropColumn(
                name: "OccupationalHygiene",
                table: "Chemicals");

            migrationBuilder.DropColumn(
                name: "OdorThreshold",
                table: "Chemicals");

            migrationBuilder.DropColumn(
                name: "OtherEcologicalEffects",
                table: "Chemicals");

            migrationBuilder.DropColumn(
                name: "PersistenceDegradability",
                table: "Chemicals");

            migrationBuilder.DropColumn(
                name: "PossibilityOfHazardousReactions",
                table: "Chemicals");

            migrationBuilder.DropColumn(
                name: "ProductCode",
                table: "Chemicals");

            migrationBuilder.DropColumn(
                name: "Reactivity",
                table: "Chemicals");

            migrationBuilder.DropColumn(
                name: "RoutesOfExposure",
                table: "Chemicals");

            migrationBuilder.DropColumn(
                name: "SARA302304",
                table: "Chemicals");

            migrationBuilder.DropColumn(
                name: "SubstanceMixture",
                table: "Chemicals");

            migrationBuilder.DropColumn(
                name: "Teratogenicity",
                table: "Chemicals");

            migrationBuilder.DropColumn(
                name: "TransportSpecialPrecautions",
                table: "Chemicals");

            migrationBuilder.DropColumn(
                name: "pH",
                table: "Chemicals");
        }
    }
}
