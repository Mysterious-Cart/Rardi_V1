using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CHKS.Migrations
{
    /// <inheritdoc />
    public partial class ModifiedCart : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_cart_car_Plate",
                table: "cart");

            migrationBuilder.DropForeignKey(
                name: "FK_connector_inventory_InventoryCode",
                table: "connector");

            migrationBuilder.DropForeignKey(
                name: "FK_historyconnector_inventory_InventoryCode",
                table: "historyconnector");

            migrationBuilder.DropPrimaryKey(
                name: "PK_inventory",
                table: "inventory");

            migrationBuilder.DropIndex(
                name: "IX_historyconnector_InventoryCode",
                table: "historyconnector");

            migrationBuilder.DropIndex(
                name: "IX_connector_InventoryCode",
                table: "connector");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "inventory");

            migrationBuilder.DropColumn(
                name: "InventoryCode",
                table: "historyconnector");

            migrationBuilder.DropColumn(
                name: "InventoryCode",
                table: "connector");

            migrationBuilder.DropColumn(
                name: "Company",
                table: "cart");

            migrationBuilder.DropColumn(
                name: "Creator",
                table: "cart");

            migrationBuilder.RenameColumn(
                name: "Plate",
                table: "cart",
                newName: "Car_Id");

            migrationBuilder.RenameIndex(
                name: "IX_cart_Plate",
                table: "cart",
                newName: "IX_cart_Car_Id");

            migrationBuilder.AddColumn<Guid>(
                name: "InventoryId",
                table: "historyconnector",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.AddColumn<Guid>(
                name: "InventoryId",
                table: "connector",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.AlterColumn<decimal>(
                name: "Total",
                table: "cart",
                type: "decimal(10,2)",
                precision: 10,
                scale: 2,
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)",
                oldPrecision: 10,
                oldScale: 2,
                oldNullable: true);

            migrationBuilder.AddColumn<short>(
                name: "Status",
                table: "cart",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_inventory",
                table: "inventory",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_historyconnector_InventoryId",
                table: "historyconnector",
                column: "InventoryId");

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

            migrationBuilder.AddForeignKey(
                name: "FK_historyconnector_inventory_InventoryId",
                table: "historyconnector",
                column: "InventoryId",
                principalTable: "inventory",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_cart_car_Car_Id",
                table: "cart");

            migrationBuilder.DropForeignKey(
                name: "FK_connector_inventory_InventoryId",
                table: "connector");

            migrationBuilder.DropForeignKey(
                name: "FK_historyconnector_inventory_InventoryId",
                table: "historyconnector");

            migrationBuilder.DropPrimaryKey(
                name: "PK_inventory",
                table: "inventory");

            migrationBuilder.DropIndex(
                name: "IX_historyconnector_InventoryId",
                table: "historyconnector");

            migrationBuilder.DropIndex(
                name: "IX_connector_InventoryId",
                table: "connector");

            migrationBuilder.DropColumn(
                name: "InventoryId",
                table: "historyconnector");

            migrationBuilder.DropColumn(
                name: "InventoryId",
                table: "connector");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "cart");

            migrationBuilder.RenameColumn(
                name: "Car_Id",
                table: "cart",
                newName: "Plate");

            migrationBuilder.RenameIndex(
                name: "IX_cart_Car_Id",
                table: "cart",
                newName: "IX_cart_Plate");

            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "inventory",
                type: "varchar(255)",
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "InventoryCode",
                table: "historyconnector",
                type: "varchar(255)",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "InventoryCode",
                table: "connector",
                type: "varchar(255)",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<decimal>(
                name: "Total",
                table: "cart",
                type: "decimal(10,2)",
                precision: 10,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)",
                oldPrecision: 10,
                oldScale: 2);

            migrationBuilder.AddColumn<short>(
                name: "Company",
                table: "cart",
                type: "smallint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Creator",
                table: "cart",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddPrimaryKey(
                name: "PK_inventory",
                table: "inventory",
                column: "Code");

            migrationBuilder.CreateIndex(
                name: "IX_historyconnector_InventoryCode",
                table: "historyconnector",
                column: "InventoryCode");

            migrationBuilder.CreateIndex(
                name: "IX_connector_InventoryCode",
                table: "connector",
                column: "InventoryCode");

            migrationBuilder.AddForeignKey(
                name: "FK_cart_car_Plate",
                table: "cart",
                column: "Plate",
                principalTable: "car",
                principalColumn: "Plate");

            migrationBuilder.AddForeignKey(
                name: "FK_connector_inventory_InventoryCode",
                table: "connector",
                column: "InventoryCode",
                principalTable: "inventory",
                principalColumn: "Code");

            migrationBuilder.AddForeignKey(
                name: "FK_historyconnector_inventory_InventoryCode",
                table: "historyconnector",
                column: "InventoryCode",
                principalTable: "inventory",
                principalColumn: "Code");
        }
    }
}
