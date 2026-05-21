using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChemicalSDS.Migrations
{
    /// <inheritdoc />
    public partial class Section12And14Extended : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TransportBulkMarpol",
                table: "Chemicals",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TransportDotClassification",
                table: "Chemicals",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TransportIataClassification",
                table: "Chemicals",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TransportRegulations",
                table: "Chemicals",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TransportTdgClassification",
                table: "Chemicals",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Chemicals",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "TransportBulkMarpol", "TransportDotClassification", "TransportIataClassification", "TransportRegulations", "TransportTdgClassification" },
                values: new object[] { null, null, null, null, null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TransportBulkMarpol",
                table: "Chemicals");

            migrationBuilder.DropColumn(
                name: "TransportDotClassification",
                table: "Chemicals");

            migrationBuilder.DropColumn(
                name: "TransportIataClassification",
                table: "Chemicals");

            migrationBuilder.DropColumn(
                name: "TransportRegulations",
                table: "Chemicals");

            migrationBuilder.DropColumn(
                name: "TransportTdgClassification",
                table: "Chemicals");
        }
    }
}
