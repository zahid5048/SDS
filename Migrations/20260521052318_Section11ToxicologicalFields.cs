using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChemicalSDS.Migrations
{
    /// <inheritdoc />
    public partial class Section11ToxicologicalFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AcuteHealthEffectsEye",
                table: "Chemicals",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AcuteHealthEffectsIngestion",
                table: "Chemicals",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AcuteHealthEffectsInhalation",
                table: "Chemicals",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AcuteHealthEffectsSkin",
                table: "Chemicals",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AcuteToxicityEstimates",
                table: "Chemicals",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AspirationHazard",
                table: "Chemicals",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CarcinogenicityClassification",
                table: "Chemicals",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ChronicCarcinogenicity",
                table: "Chemicals",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ChronicEffectsGeneral",
                table: "Chemicals",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ChronicMutagenicity",
                table: "Chemicals",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ChronicTeratogenicity",
                table: "Chemicals",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DevelopmentalEffects",
                table: "Chemicals",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FertilityEffects",
                table: "Chemicals",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LongTermDelayedEffects",
                table: "Chemicals",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LongTermImmediateEffects",
                table: "Chemicals",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReproductiveToxicity",
                table: "Chemicals",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Sensitization",
                table: "Chemicals",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ShortTermDelayedEffects",
                table: "Chemicals",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ShortTermImmediateEffects",
                table: "Chemicals",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TargetOrganToxicityRepeated",
                table: "Chemicals",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TargetOrganToxicitySingle",
                table: "Chemicals",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ToxicologicalSymptomsEye",
                table: "Chemicals",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ToxicologicalSymptomsIngestion",
                table: "Chemicals",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ToxicologicalSymptomsInhalation",
                table: "Chemicals",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ToxicologicalSymptomsSkin",
                table: "Chemicals",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Chemicals",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "AcuteHealthEffectsEye", "AcuteHealthEffectsIngestion", "AcuteHealthEffectsInhalation", "AcuteHealthEffectsSkin", "AcuteToxicityEstimates", "AspirationHazard", "CarcinogenicityClassification", "ChronicCarcinogenicity", "ChronicEffectsGeneral", "ChronicMutagenicity", "ChronicTeratogenicity", "DevelopmentalEffects", "FertilityEffects", "LongTermDelayedEffects", "LongTermImmediateEffects", "ReproductiveToxicity", "Sensitization", "ShortTermDelayedEffects", "ShortTermImmediateEffects", "TargetOrganToxicityRepeated", "TargetOrganToxicitySingle", "ToxicologicalSymptomsEye", "ToxicologicalSymptomsIngestion", "ToxicologicalSymptomsInhalation", "ToxicologicalSymptomsSkin" },
                values: new object[] { null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AcuteHealthEffectsEye",
                table: "Chemicals");

            migrationBuilder.DropColumn(
                name: "AcuteHealthEffectsIngestion",
                table: "Chemicals");

            migrationBuilder.DropColumn(
                name: "AcuteHealthEffectsInhalation",
                table: "Chemicals");

            migrationBuilder.DropColumn(
                name: "AcuteHealthEffectsSkin",
                table: "Chemicals");

            migrationBuilder.DropColumn(
                name: "AcuteToxicityEstimates",
                table: "Chemicals");

            migrationBuilder.DropColumn(
                name: "AspirationHazard",
                table: "Chemicals");

            migrationBuilder.DropColumn(
                name: "CarcinogenicityClassification",
                table: "Chemicals");

            migrationBuilder.DropColumn(
                name: "ChronicCarcinogenicity",
                table: "Chemicals");

            migrationBuilder.DropColumn(
                name: "ChronicEffectsGeneral",
                table: "Chemicals");

            migrationBuilder.DropColumn(
                name: "ChronicMutagenicity",
                table: "Chemicals");

            migrationBuilder.DropColumn(
                name: "ChronicTeratogenicity",
                table: "Chemicals");

            migrationBuilder.DropColumn(
                name: "DevelopmentalEffects",
                table: "Chemicals");

            migrationBuilder.DropColumn(
                name: "FertilityEffects",
                table: "Chemicals");

            migrationBuilder.DropColumn(
                name: "LongTermDelayedEffects",
                table: "Chemicals");

            migrationBuilder.DropColumn(
                name: "LongTermImmediateEffects",
                table: "Chemicals");

            migrationBuilder.DropColumn(
                name: "ReproductiveToxicity",
                table: "Chemicals");

            migrationBuilder.DropColumn(
                name: "Sensitization",
                table: "Chemicals");

            migrationBuilder.DropColumn(
                name: "ShortTermDelayedEffects",
                table: "Chemicals");

            migrationBuilder.DropColumn(
                name: "ShortTermImmediateEffects",
                table: "Chemicals");

            migrationBuilder.DropColumn(
                name: "TargetOrganToxicityRepeated",
                table: "Chemicals");

            migrationBuilder.DropColumn(
                name: "TargetOrganToxicitySingle",
                table: "Chemicals");

            migrationBuilder.DropColumn(
                name: "ToxicologicalSymptomsEye",
                table: "Chemicals");

            migrationBuilder.DropColumn(
                name: "ToxicologicalSymptomsIngestion",
                table: "Chemicals");

            migrationBuilder.DropColumn(
                name: "ToxicologicalSymptomsInhalation",
                table: "Chemicals");

            migrationBuilder.DropColumn(
                name: "ToxicologicalSymptomsSkin",
                table: "Chemicals");
        }
    }
}
