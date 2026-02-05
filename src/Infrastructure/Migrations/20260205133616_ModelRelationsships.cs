using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ModelRelationsships : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Wines_StorageUnits_StorageUnitId",
                table: "Wines");

            migrationBuilder.AlterColumn<string>(
                name: "Wineyard",
                table: "Wines",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "Wines",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Wines",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "StorageName",
                table: "StorageUnits",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<Guid>(
                name: "CellarId",
                table: "StorageUnits",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "Cellars",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_StorageUnits_CellarId",
                table: "StorageUnits",
                column: "CellarId");

            migrationBuilder.AddForeignKey(
                name: "FK_StorageUnits_Cellars_CellarId",
                table: "StorageUnits",
                column: "CellarId",
                principalTable: "Cellars",
                principalColumn: "CellarId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Wines_StorageUnits_StorageUnitId",
                table: "Wines",
                column: "StorageUnitId",
                principalTable: "StorageUnits",
                principalColumn: "StorageUnitId",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StorageUnits_Cellars_CellarId",
                table: "StorageUnits");

            migrationBuilder.DropForeignKey(
                name: "FK_Wines_StorageUnits_StorageUnitId",
                table: "Wines");

            migrationBuilder.DropIndex(
                name: "IX_StorageUnits_CellarId",
                table: "StorageUnits");

            migrationBuilder.DropColumn(
                name: "CellarId",
                table: "StorageUnits");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Cellars");

            migrationBuilder.AlterColumn<string>(
                name: "Wineyard",
                table: "Wines",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "Wines",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(30)",
                oldMaxLength: 30);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Wines",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(30)",
                oldMaxLength: 30);

            migrationBuilder.AlterColumn<string>(
                name: "StorageName",
                table: "StorageUnits",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AddForeignKey(
                name: "FK_Wines_StorageUnits_StorageUnitId",
                table: "Wines",
                column: "StorageUnitId",
                principalTable: "StorageUnits",
                principalColumn: "StorageUnitId");
        }
    }
}
