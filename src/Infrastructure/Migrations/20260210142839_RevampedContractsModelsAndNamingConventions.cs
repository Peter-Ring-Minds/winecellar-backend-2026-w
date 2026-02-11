using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RevampedContractsModelsAndNamingConventions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Wines_StorageUnits_StorageUnitId",
                table: "Wines");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Wines",
                table: "Wines");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StorageUnits",
                table: "StorageUnits");

            migrationBuilder.RenameColumn(
                name: "WineId",
                table: "Wines",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "StorageUnitName",
                table: "StorageUnits",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "StorageUnitId",
                table: "StorageUnits",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "CellarId",
                table: "Cellars",
                newName: "Id");

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "Wines",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Wines",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "StorageUnits",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "StorageUnits",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Cellars",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Cellars",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Wines",
                table: "Wines",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StorageUnits",
                table: "StorageUnits",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Wines_StorageUnits_StorageUnitId",
                table: "Wines",
                column: "StorageUnitId",
                principalTable: "StorageUnits",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Wines_StorageUnits_StorageUnitId",
                table: "Wines");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Wines",
                table: "Wines");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StorageUnits",
                table: "StorageUnits");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Wines");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Wines");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "StorageUnits");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "StorageUnits");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Cellars");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Cellars");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Wines",
                newName: "WineId");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "StorageUnits",
                newName: "StorageUnitId");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "StorageUnits",
                newName: "StorageUnitName");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Cellars",
                newName: "CellarId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Wines",
                table: "Wines",
                column: "WineId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StorageUnits",
                table: "StorageUnits",
                column: "StorageUnitId");

            migrationBuilder.AddForeignKey(
                name: "FK_Wines_StorageUnits_StorageUnitId",
                table: "Wines",
                column: "StorageUnitId",
                principalTable: "StorageUnits",
                principalColumn: "StorageUnitId",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
