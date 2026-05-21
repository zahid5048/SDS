using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChemicalSDS.Migrations
{
    /// <inheritdoc />
    public partial class SdsSectionsExtendedFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CriticalTemperature",
                table: "Chemicals",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DecompositionTemperature",
                table: "Chemicals",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EnvironmentalExposureControls",
                table: "Chemicals",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EnvironmentalPrecautionsSpill",
                table: "Chemicals",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FirefighterProtectiveActions",
                table: "Chemicals",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FirefighterProtectiveEquipment",
                table: "Chemicals",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FlowTimeIso2431",
                table: "Chemicals",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GasDensity",
                table: "Chemicals",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GhsPictogramCodes",
                table: "Chemicals",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "GhsPictogramImagePath",
                table: "Chemicals",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HygieneMeasures",
                table: "Chemicals",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MedicalAttentionIndication",
                table: "Chemicals",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NotesToPhysician",
                table: "Chemicals",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OtherSkinProtection",
                table: "Chemicals",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PartitionCoefficient",
                table: "Chemicals",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProtectionOfFirstAiders",
                table: "Chemicals",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SolubilityInWater",
                table: "Chemicals",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SpecificTreatments",
                table: "Chemicals",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SpecificVolume",
                table: "Chemicals",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SpillEmergencyResponders",
                table: "Chemicals",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SpillNonEmergencyPersonnel",
                table: "Chemicals",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Viscosity",
                table: "Chemicals",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Chemicals",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CriticalTemperature", "DecompositionTemperature", "EnvironmentalExposureControls", "EnvironmentalPrecautionsSpill", "FirefighterProtectiveActions", "FirefighterProtectiveEquipment", "FlowTimeIso2431", "GasDensity", "GhsPictogramCodes", "GhsPictogramImagePath", "HygieneMeasures", "MedicalAttentionIndication", "NotesToPhysician", "OtherSkinProtection", "PartitionCoefficient", "ProtectionOfFirstAiders", "SolubilityInWater", "SpecificTreatments", "SpecificVolume", "SpillEmergencyResponders", "SpillNonEmergencyPersonnel", "Viscosity" },
                values: new object[] { null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CriticalTemperature",
                table: "Chemicals");

            migrationBuilder.DropColumn(
                name: "DecompositionTemperature",
                table: "Chemicals");

            migrationBuilder.DropColumn(
                name: "EnvironmentalExposureControls",
                table: "Chemicals");

            migrationBuilder.DropColumn(
                name: "EnvironmentalPrecautionsSpill",
                table: "Chemicals");

            migrationBuilder.DropColumn(
                name: "FirefighterProtectiveActions",
                table: "Chemicals");

            migrationBuilder.DropColumn(
                name: "FirefighterProtectiveEquipment",
                table: "Chemicals");

            migrationBuilder.DropColumn(
                name: "FlowTimeIso2431",
                table: "Chemicals");

            migrationBuilder.DropColumn(
                name: "GasDensity",
                table: "Chemicals");

            migrationBuilder.DropColumn(
                name: "GhsPictogramCodes",
                table: "Chemicals");

            migrationBuilder.DropColumn(
                name: "GhsPictogramImagePath",
                table: "Chemicals");

            migrationBuilder.DropColumn(
                name: "HygieneMeasures",
                table: "Chemicals");

            migrationBuilder.DropColumn(
                name: "MedicalAttentionIndication",
                table: "Chemicals");

            migrationBuilder.DropColumn(
                name: "NotesToPhysician",
                table: "Chemicals");

            migrationBuilder.DropColumn(
                name: "OtherSkinProtection",
                table: "Chemicals");

            migrationBuilder.DropColumn(
                name: "PartitionCoefficient",
                table: "Chemicals");

            migrationBuilder.DropColumn(
                name: "ProtectionOfFirstAiders",
                table: "Chemicals");

            migrationBuilder.DropColumn(
                name: "SolubilityInWater",
                table: "Chemicals");

            migrationBuilder.DropColumn(
                name: "SpecificTreatments",
                table: "Chemicals");

            migrationBuilder.DropColumn(
                name: "SpecificVolume",
                table: "Chemicals");

            migrationBuilder.DropColumn(
                name: "SpillEmergencyResponders",
                table: "Chemicals");

            migrationBuilder.DropColumn(
                name: "SpillNonEmergencyPersonnel",
                table: "Chemicals");

            migrationBuilder.DropColumn(
                name: "Viscosity",
                table: "Chemicals");
        }
    }
}
