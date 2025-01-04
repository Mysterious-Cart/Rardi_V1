using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CHKS.Migrations
{
    /// <inheritdoc />
    public partial class changeInventoryForgeinkey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_connector_inventory_Product",
                table: "connector");

            migrationBuilder.DropForeignKey(
                name: "FK_historyconnector_inventory_Product",
                table: "historyconnector");

            migrationBuilder.AlterColumn<Guid>(
                name: "Product",
                table: "historyconnector",
                type: "char(36)",
                nullable: false,
                collation: "ascii_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(255)")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<Guid>(
                name: "Product",
                table: "connector",
                type: "char(36)",
                nullable: false,
                collation: "ascii_general_ci",
                oldClrType: typeof(string),
                oldType: "varchar(255)")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_inventory_Id",
                table: "inventory",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_connector_inventory_Product",
                table: "connector",
                column: "Product",
                principalTable: "inventory",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_historyconnector_inventory_Product",
                table: "historyconnector",
                column: "Product",
                principalTable: "inventory",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_connector_inventory_Product",
                table: "connector");

            migrationBuilder.DropForeignKey(
                name: "FK_historyconnector_inventory_Product",
                table: "historyconnector");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_inventory_Id",
                table: "inventory");

            migrationBuilder.AlterColumn<string>(
                name: "Product",
                table: "historyconnector",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "char(36)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AlterColumn<string>(
                name: "Product",
                table: "connector",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "char(36)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("Relational:Collation", "ascii_general_ci");

            migrationBuilder.AddForeignKey(
                name: "FK_connector_inventory_Product",
                table: "connector",
                column: "Product",
                principalTable: "inventory",
                principalColumn: "Code");

            migrationBuilder.AddForeignKey(
                name: "FK_historyconnector_inventory_Product",
                table: "historyconnector",
                column: "Product",
                principalTable: "inventory",
                principalColumn: "Code");
        }
    }
}
