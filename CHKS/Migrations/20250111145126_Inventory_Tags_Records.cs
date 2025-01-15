using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CHKS.Migrations
{
    /// <inheritdoc />
    public partial class Inventory_Tags_Records : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_historyconnector_inventory_InventoryId",
                table: "historyconnector");

            migrationBuilder.DropIndex(
                name: "IX_historyconnector_InventoryId",
                table: "historyconnector");

            migrationBuilder.DropColumn(
                name: "InventoryId",
                table: "historyconnector");

            migrationBuilder.AddColumn<string>(
                name: "Color",
                table: "Tags",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Tag",
                table: "Tags",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "Optimal_Stock",
                table: "inventory",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Sold_Total",
                table: "inventory",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<decimal>(
                name: "Qty",
                table: "historyconnector",
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

            migrationBuilder.AlterColumn<decimal>(
                name: "Export",
                table: "historyconnector",
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

            migrationBuilder.AlterColumn<decimal>(
                name: "Expense",
                table: "dailyexpense",
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

            migrationBuilder.AlterColumn<Guid>(
                name: "Key",
                table: "dailyexpense",
                type: "char(36)",
                nullable: false,
                collation: "ascii_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(255)")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "Date",
                table: "dailyexpense",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<short>(
                name: "IsDeleted",
                table: "car",
                type: "smallint",
                nullable: false,
                defaultValueSql: "'0'",
                oldClrType: typeof(short),
                oldType: "smallint",
                oldNullable: true,
                oldDefaultValueSql: "'0'");

            migrationBuilder.CreateIndex(
                name: "IX_historyconnector_ProductId",
                table: "historyconnector",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_historyconnector_inventory_ProductId",
                table: "historyconnector",
                column: "ProductId",
                principalTable: "inventory",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_historyconnector_inventory_ProductId",
                table: "historyconnector");

            migrationBuilder.DropIndex(
                name: "IX_historyconnector_ProductId",
                table: "historyconnector");

            migrationBuilder.DropColumn(
                name: "Color",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "Tag",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "Optimal_Stock",
                table: "inventory");

            migrationBuilder.DropColumn(
                name: "Sold_Total",
                table: "inventory");

            migrationBuilder.DropColumn(
                name: "Date",
                table: "dailyexpense");

            migrationBuilder.AlterColumn<decimal>(
                name: "Qty",
                table: "historyconnector",
                type: "decimal(10,2)",
                precision: 10,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)",
                oldPrecision: 10,
                oldScale: 2);

            migrationBuilder.AlterColumn<decimal>(
                name: "Export",
                table: "historyconnector",
                type: "decimal(10,2)",
                precision: 10,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)",
                oldPrecision: 10,
                oldScale: 2);

            migrationBuilder.AddColumn<Guid>(
                name: "InventoryId",
                table: "historyconnector",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.AlterColumn<decimal>(
                name: "Expense",
                table: "dailyexpense",
                type: "decimal(10,2)",
                precision: 10,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)",
                oldPrecision: 10,
                oldScale: 2);

            migrationBuilder.AlterColumn<string>(
                name: "Key",
                table: "dailyexpense",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "char(36)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<short>(
                name: "IsDeleted",
                table: "car",
                type: "smallint",
                nullable: true,
                defaultValueSql: "'0'",
                oldClrType: typeof(short),
                oldType: "smallint",
                oldDefaultValueSql: "'0'");

            migrationBuilder.CreateIndex(
                name: "IX_historyconnector_InventoryId",
                table: "historyconnector",
                column: "InventoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_historyconnector_inventory_InventoryId",
                table: "historyconnector",
                column: "InventoryId",
                principalTable: "inventory",
                principalColumn: "Id");
        }
    }
}
