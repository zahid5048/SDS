using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChemicalSDS.Migrations
{
    /// <inheritdoc />
    public partial class Sec02BlocksJson : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Sec02Blocks",
                table: "Chemicals",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Chemicals",
                keyColumn: "Id",
                keyValue: 1,
                column: "Sec02Blocks",
                value: null);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Sec02Blocks",
                table: "Chemicals");
        }
    }
}
