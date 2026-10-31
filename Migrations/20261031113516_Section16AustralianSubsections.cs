using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChemicalSDS.Migrations
{
    /// <inheritdoc />
    public partial class Section16AustralianSubsections : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Sec16FullTextGhsClassifications",
                table: "Chemicals",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Sec16FullTextHPhrases",
                table: "Chemicals",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Sec16RecommendedApplications",
                table: "Chemicals",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Sec16RevisionPreparedFor",
                table: "Chemicals",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Sec16FullTextGhsClassifications",
                table: "Chemicals");

            migrationBuilder.DropColumn(
                name: "Sec16FullTextHPhrases",
                table: "Chemicals");

            migrationBuilder.DropColumn(
                name: "Sec16RecommendedApplications",
                table: "Chemicals");

            migrationBuilder.DropColumn(
                name: "Sec16RevisionPreparedFor",
                table: "Chemicals");
        }
    }
}
