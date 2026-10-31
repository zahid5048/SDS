using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChemicalSDS.Migrations
{
    /// <inheritdoc />
    public partial class Sec11SubsectionsJson : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Sec11Subsections",
                table: "Chemicals",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Chemicals",
                keyColumn: "Id",
                keyValue: 1,
                column: "Sec11Subsections",
                value: null);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Sec11Subsections",
                table: "Chemicals");
        }
    }
}
