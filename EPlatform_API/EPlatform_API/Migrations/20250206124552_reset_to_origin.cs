using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EPlatform_API.Migrations
{
    /// <inheritdoc />
    public partial class reset_to_origin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_Inventories_InventoryId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_InventoryId",
                table: "Products");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Inventories",
                table: "Inventories");

            migrationBuilder.DropColumn(
                name: "InventoryId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "InventoryId",
                table: "Inventories");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Inventories",
                table: "Inventories",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_Inventories_Products_ProductId",
                table: "Inventories",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "ProductId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Inventories_Products_ProductId",
                table: "Inventories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Inventories",
                table: "Inventories");

            migrationBuilder.AddColumn<int>(
                name: "InventoryId",
                table: "Products",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "InventoryId",
                table: "Inventories",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Inventories",
                table: "Inventories",
                column: "InventoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_InventoryId",
                table: "Products",
                column: "InventoryId",
                unique: true,
                filter: "[InventoryId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Inventories_InventoryId",
                table: "Products",
                column: "InventoryId",
                principalTable: "Inventories",
                principalColumn: "InventoryId",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
