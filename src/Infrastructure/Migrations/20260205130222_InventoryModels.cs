using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InventoryModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Cellars",
                columns: table => new
                {
                    CellarId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cellars", x => x.CellarId);
                });

            migrationBuilder.CreateTable(
                name: "StorageUnits",
                columns: table => new
                {
                    StorageUnitId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StorageName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StorageUnits", x => x.StorageUnitId);
                });

            migrationBuilder.CreateTable(
                name: "Wines",
                columns: table => new
                {
                    WineId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Wineyard = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Vintage = table.Column<int>(type: "int", nullable: false),
                    StorageUnitId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Wines", x => x.WineId);
                    table.ForeignKey(
                        name: "FK_Wines_StorageUnits_StorageUnitId",
                        column: x => x.StorageUnitId,
                        principalTable: "StorageUnits",
                        principalColumn: "StorageUnitId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Wines_StorageUnitId",
                table: "Wines",
                column: "StorageUnitId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Cellars");

            migrationBuilder.DropTable(
                name: "Wines");

            migrationBuilder.DropTable(
                name: "StorageUnits");
        }
    }
}
