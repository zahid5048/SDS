using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChemicalSDS.Migrations
{
    /// <inheritdoc />
    public partial class SyncSection16PendingModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Chemicals",
                keyColumn: "Id",
                keyValue: 1,
                column: "NoticeToReader",
                value: "To the best of our knowledge, the information contained herein is accurate.");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Chemicals",
                keyColumn: "Id",
                keyValue: 1,
                column: "NoticeToReader",
                value: "To the best of our knowledge, the information contained herein is accurate. However, neither the above-named supplier, nor any of its subsidiaries, assumes any liability whatsoever for the accuracy or completeness of the information contained herein. Final determination of suitability of any material is the sole responsibility of the user. All materials may present unknown hazards and should be used with caution. Although certain hazards are described herein, we cannot guarantee that these are the only hazards that exist.");
        }
    }
}
