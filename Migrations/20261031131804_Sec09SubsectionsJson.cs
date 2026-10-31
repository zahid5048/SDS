using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChemicalSDS.Migrations
{
    /// <inheritdoc />
    public partial class Sec09SubsectionsJson : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Sec09SolventContent",
                table: "Chemicals",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Sec09Subsections",
                table: "Chemicals",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Sec09Voc",
                table: "Chemicals",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Chemicals",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Sec09SolventContent", "Sec09Subsections", "Sec09Voc" },
                values: new object[] { null, null, null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Sec09SolventContent",
                table: "Chemicals");

            migrationBuilder.DropColumn(
                name: "Sec09Subsections",
                table: "Chemicals");

            migrationBuilder.DropColumn(
                name: "Sec09Voc",
                table: "Chemicals");
        }
    }
}
