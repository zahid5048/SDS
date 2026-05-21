using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChemicalSDS.Migrations
{
    /// <inheritdoc />
    public partial class SyncPendingModelChanges2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "HazardousPolymerization",
                table: "Chemicals",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Chemicals",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "HazardousPolymerization", "IrritationCorrosion" },
                values: new object[] { "Under normal conditions of storage and use, hazardous polymerization will not occur.", "ethanol|Eyes - Mild irritant|Rabbit|-|24 hours 500 milligrams|-\nethanol|Eyes - Moderate irritant|Rabbit|-|0.066666667 minutes 100 milligrams|-\nethanol|Eyes - Moderate irritant|Rabbit|-|100 microliters|-\nethanol|Eyes - Severe irritant|Rabbit|-|500 milligrams|-\nethanol|Skin - Mild irritant|Rabbit|-|400 milligrams|-\nethanol|Skin - Moderate irritant|Rabbit|-|24 hours 20 milligrams|-" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HazardousPolymerization",
                table: "Chemicals");

            migrationBuilder.UpdateData(
                table: "Chemicals",
                keyColumn: "Id",
                keyValue: 1,
                column: "IrritationCorrosion",
                value: "Eyes - Mild irritant Rabbit - 500 milligrams. Skin - Mild irritant Rabbit - 400 milligrams.");
        }
    }
}
