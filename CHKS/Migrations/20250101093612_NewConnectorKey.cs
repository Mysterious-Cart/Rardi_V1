using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CHKS.Migrations
{
    /// <inheritdoc />
    public partial class NewConnectorKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_cart_car_Car_Id",
                table: "cart");

            migrationBuilder.DropForeignKey(
                name: "FK_connector_inventory_InventoryId",
                table: "connector");

            migrationBuilder.DropPrimaryKey(
                name: "PK_connector",
                table: "connector");

            migrationBuilder.DropIndex(
                name: "IX_connector_InventoryId",
                table: "connector");

            migrationBuilder.DropColumn(
                name: "GeneratedKey",
                table: "connector");

            migrationBuilder.DropColumn(
                name: "InventoryId",
                table: "connector");

            migrationBuilder.RenameColumn(
                name: "Car_Id",
                table: "cart",
                newName: "CarID");

            migrationBuilder.RenameIndex(
                name: "IX_cart_Car_Id",
                table: "cart",
                newName: "IX_cart_CarID");

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "connector",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                collation: "ascii_general_ci");

            migrationBuilder.AddPrimaryKey(
                name: "PK_connector",
                table: "connector",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_cart_car_CarID",
                table: "cart",
                column: "CarID",
                principalTable: "car",
                principalColumn: "Plate");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_cart_car_CarID",
                table: "cart");

            migrationBuilder.DropPrimaryKey(
                name: "PK_connector",
                table: "connector");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "connector");

            migrationBuilder.RenameColumn(
                name: "CarID",
                table: "cart",
                newName: "Car_Id");

            migrationBuilder.RenameIndex(
                name: "IX_cart_CarID",
                table: "cart",
                newName: "IX_cart_Car_Id");

            migrationBuilder.AddColumn<string>(
                name: "GeneratedKey",
                table: "connector",
                type: "varchar(255)",
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<Guid>(
                name: "InventoryId",
                table: "connector",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.AddPrimaryKey(
                name: "PK_connector",
                table: "connector",
                column: "GeneratedKey");

            migrationBuilder.CreateIndex(
                name: "IX_connector_InventoryId",
                table: "connector",
                column: "InventoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_cart_car_Car_Id",
                table: "cart",
                column: "Car_Id",
                principalTable: "car",
                principalColumn: "Plate");

            migrationBuilder.AddForeignKey(
                name: "FK_connector_inventory_InventoryId",
                table: "connector",
                column: "InventoryId",
                principalTable: "inventory",
                principalColumn: "Id");
        }
    }
}
