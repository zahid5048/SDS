using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChemicalSDS.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Chemicals",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductIdentifier = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Sec01Subsections = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Sec02Blocks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Sec03Blocks = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Sec04Subsections = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Sec05Subsections = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Sec06Subsections = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Sec07Subsections = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Sec08Subsections = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Sec09Subsections = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Sec10Subsections = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Sec11Subsections = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Sec12Subsections = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Sec13Subsections = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Sec14Subsections = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Sec15Subsections = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Sec16Subsections = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastCompletedSection = table.Column<int>(type: "int", nullable: false),
                    IsDraft = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Chemicals", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Username = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    LastLoginDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Chemicals");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
