using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChemicalSDS.Migrations
{
    /// <inheritdoc />
    public partial class AddSec01ManufacturerColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                IF NOT EXISTS (
                    SELECT 1 FROM sys.columns
                    WHERE object_id = OBJECT_ID(N'[Chemicals]') AND name = N'Sec01Manufacturer')
                ALTER TABLE [Chemicals] ADD [Sec01Manufacturer] nvarchar(max) NULL;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Sec01Manufacturer",
                table: "Chemicals");
        }
    }
}
