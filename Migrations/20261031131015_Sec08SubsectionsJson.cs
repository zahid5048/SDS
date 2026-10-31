using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChemicalSDS.Migrations
{
    /// <inheritdoc />
    public partial class Sec08SubsectionsJson : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Sec08Subsections",
                table: "Chemicals",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Chemicals",
                keyColumn: "Id",
                keyValue: 1,
                column: "Sec08Subsections",
                value: null);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Sec08Subsections",
                table: "Chemicals");
        }
    }
}
