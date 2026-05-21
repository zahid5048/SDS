using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChemicalSDS.Migrations
{
    /// <inheritdoc />
    public partial class Section15RegulatoryFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RegChemWeaponConvention",
                table: "Chemicals",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RegCleanAirActClassI",
                table: "Chemicals",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RegCleanAirActClassII",
                table: "Chemicals",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RegCleanAirActHaps",
                table: "Chemicals",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RegDeaListI",
                table: "Chemicals",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RegDeaListII",
                table: "Chemicals",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RegInventoryAustralia",
                table: "Chemicals",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RegInventoryCanada",
                table: "Chemicals",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RegInventoryChina",
                table: "Chemicals",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RegInventoryEurope",
                table: "Chemicals",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RegInventoryJapanEncs",
                table: "Chemicals",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RegInventoryJapanIshl",
                table: "Chemicals",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RegInventoryKorea",
                table: "Chemicals",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RegInventoryMalaysia",
                table: "Chemicals",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RegInventoryNewZealand",
                table: "Chemicals",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RegInventoryPhilippines",
                table: "Chemicals",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RegMontrealProtocol",
                table: "Chemicals",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RegRotterdamConvention",
                table: "Chemicals",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RegSara302Composition",
                table: "Chemicals",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RegSara304RQ",
                table: "Chemicals",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RegSara311312Classification",
                table: "Chemicals",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RegStateMassachusetts",
                table: "Chemicals",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RegStateNewJersey",
                table: "Chemicals",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RegStateNewYork",
                table: "Chemicals",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RegStatePennsylvania",
                table: "Chemicals",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RegStockholmConvention",
                table: "Chemicals",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RegTscaCdrExempt",
                table: "Chemicals",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RegUneceAarhus",
                table: "Chemicals",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Chemicals",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "RegChemWeaponConvention", "RegCleanAirActClassI", "RegCleanAirActClassII", "RegCleanAirActHaps", "RegDeaListI", "RegDeaListII", "RegInventoryAustralia", "RegInventoryCanada", "RegInventoryChina", "RegInventoryEurope", "RegInventoryJapanEncs", "RegInventoryJapanIshl", "RegInventoryKorea", "RegInventoryMalaysia", "RegInventoryNewZealand", "RegInventoryPhilippines", "RegMontrealProtocol", "RegRotterdamConvention", "RegSara302Composition", "RegSara304RQ", "RegSara311312Classification", "RegStateMassachusetts", "RegStateNewJersey", "RegStateNewYork", "RegStatePennsylvania", "RegStockholmConvention", "RegTscaCdrExempt", "RegUneceAarhus" },
                values: new object[] { "Not listed.", "Not listed", "Not listed", "Not listed", "Not listed", "Not listed", "This material is listed or exempted.", "This material is listed or exempted.", "This material is listed or exempted.", "This material is listed or exempted.", "This material is listed or exempted.", "This material is listed or exempted.", "This material is listed or exempted.", "This material is listed or exempted.", "This material is listed or exempted.", "This material is listed or exempted.", "Not listed.", "Not listed.", "No products were found.", "Not applicable.", "Refer to Section 2: Hazards Identification of this SDS for classification of substance.", "This material is listed.", "This material is listed.", "This material is not listed.", "This material is listed.", "Not listed.", "Not determined", "Not listed." });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RegChemWeaponConvention",
                table: "Chemicals");

            migrationBuilder.DropColumn(
                name: "RegCleanAirActClassI",
                table: "Chemicals");

            migrationBuilder.DropColumn(
                name: "RegCleanAirActClassII",
                table: "Chemicals");

            migrationBuilder.DropColumn(
                name: "RegCleanAirActHaps",
                table: "Chemicals");

            migrationBuilder.DropColumn(
                name: "RegDeaListI",
                table: "Chemicals");

            migrationBuilder.DropColumn(
                name: "RegDeaListII",
                table: "Chemicals");

            migrationBuilder.DropColumn(
                name: "RegInventoryAustralia",
                table: "Chemicals");

            migrationBuilder.DropColumn(
                name: "RegInventoryCanada",
                table: "Chemicals");

            migrationBuilder.DropColumn(
                name: "RegInventoryChina",
                table: "Chemicals");

            migrationBuilder.DropColumn(
                name: "RegInventoryEurope",
                table: "Chemicals");

            migrationBuilder.DropColumn(
                name: "RegInventoryJapanEncs",
                table: "Chemicals");

            migrationBuilder.DropColumn(
                name: "RegInventoryJapanIshl",
                table: "Chemicals");

            migrationBuilder.DropColumn(
                name: "RegInventoryKorea",
                table: "Chemicals");

            migrationBuilder.DropColumn(
                name: "RegInventoryMalaysia",
                table: "Chemicals");

            migrationBuilder.DropColumn(
                name: "RegInventoryNewZealand",
                table: "Chemicals");

            migrationBuilder.DropColumn(
                name: "RegInventoryPhilippines",
                table: "Chemicals");

            migrationBuilder.DropColumn(
                name: "RegMontrealProtocol",
                table: "Chemicals");

            migrationBuilder.DropColumn(
                name: "RegRotterdamConvention",
                table: "Chemicals");

            migrationBuilder.DropColumn(
                name: "RegSara302Composition",
                table: "Chemicals");

            migrationBuilder.DropColumn(
                name: "RegSara304RQ",
                table: "Chemicals");

            migrationBuilder.DropColumn(
                name: "RegSara311312Classification",
                table: "Chemicals");

            migrationBuilder.DropColumn(
                name: "RegStateMassachusetts",
                table: "Chemicals");

            migrationBuilder.DropColumn(
                name: "RegStateNewJersey",
                table: "Chemicals");

            migrationBuilder.DropColumn(
                name: "RegStateNewYork",
                table: "Chemicals");

            migrationBuilder.DropColumn(
                name: "RegStatePennsylvania",
                table: "Chemicals");

            migrationBuilder.DropColumn(
                name: "RegStockholmConvention",
                table: "Chemicals");

            migrationBuilder.DropColumn(
                name: "RegTscaCdrExempt",
                table: "Chemicals");

            migrationBuilder.DropColumn(
                name: "RegUneceAarhus",
                table: "Chemicals");
        }
    }
}
