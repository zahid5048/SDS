using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChemicalSDS.Migrations
{
    /// <inheritdoc />
    public partial class Section15RemainingSection16 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ClassificationJustification",
                table: "Chemicals",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClassificationProcedure",
                table: "Chemicals",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HmisCautionNote",
                table: "Chemicals",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HmisFlammability",
                table: "Chemicals",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HmisHealth",
                table: "Chemicals",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HmisPhysicalHazards",
                table: "Chemicals",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NfpaCopyrightNote",
                table: "Chemicals",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NfpaFlammability",
                table: "Chemicals",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NfpaHealth",
                table: "Chemicals",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NfpaReactivity",
                table: "Chemicals",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NfpaSpecial",
                table: "Chemicals",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "References",
                table: "Chemicals",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RegInventoryTaiwan",
                table: "Chemicals",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RegInventoryThailand",
                table: "Chemicals",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RegInventoryTurkey",
                table: "Chemicals",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RegInventoryUnitedStates",
                table: "Chemicals",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RegInventoryVietnam",
                table: "Chemicals",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Chemicals",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "ClassificationJustification", "ClassificationProcedure", "HmisCautionNote", "HmisFlammability", "HmisHealth", "HmisPhysicalHazards", "NfpaCopyrightNote", "NfpaFlammability", "NfpaHealth", "NfpaReactivity", "NfpaSpecial", "References", "RegInventoryTaiwan", "RegInventoryThailand", "RegInventoryTurkey", "RegInventoryUnitedStates", "RegInventoryVietnam" },
                values: new object[] { null, null, null, null, null, null, null, null, null, null, null, null, "This material is listed or exempted.", "Not determined", "This material is listed or exempted.", "This material is listed or exempted.", "Not determined" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClassificationJustification",
                table: "Chemicals");

            migrationBuilder.DropColumn(
                name: "ClassificationProcedure",
                table: "Chemicals");

            migrationBuilder.DropColumn(
                name: "HmisCautionNote",
                table: "Chemicals");

            migrationBuilder.DropColumn(
                name: "HmisFlammability",
                table: "Chemicals");

            migrationBuilder.DropColumn(
                name: "HmisHealth",
                table: "Chemicals");

            migrationBuilder.DropColumn(
                name: "HmisPhysicalHazards",
                table: "Chemicals");

            migrationBuilder.DropColumn(
                name: "NfpaCopyrightNote",
                table: "Chemicals");

            migrationBuilder.DropColumn(
                name: "NfpaFlammability",
                table: "Chemicals");

            migrationBuilder.DropColumn(
                name: "NfpaHealth",
                table: "Chemicals");

            migrationBuilder.DropColumn(
                name: "NfpaReactivity",
                table: "Chemicals");

            migrationBuilder.DropColumn(
                name: "NfpaSpecial",
                table: "Chemicals");

            migrationBuilder.DropColumn(
                name: "References",
                table: "Chemicals");

            migrationBuilder.DropColumn(
                name: "RegInventoryTaiwan",
                table: "Chemicals");

            migrationBuilder.DropColumn(
                name: "RegInventoryThailand",
                table: "Chemicals");

            migrationBuilder.DropColumn(
                name: "RegInventoryTurkey",
                table: "Chemicals");

            migrationBuilder.DropColumn(
                name: "RegInventoryUnitedStates",
                table: "Chemicals");

            migrationBuilder.DropColumn(
                name: "RegInventoryVietnam",
                table: "Chemicals");
        }
    }
}
