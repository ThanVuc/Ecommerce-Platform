using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EPlatform_API.Migrations
{
    /// <inheritdoc />
    public partial class fix_inventory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Inventories_Warehouses_WarehouseId",
                table: "Inventories");

            migrationBuilder.RenameColumn(
                name: "WarehouseId",
                table: "Inventories",
                newName: "WareHouseId");

            migrationBuilder.RenameColumn(
                name: "ReservedQuantity",
                table: "Inventories",
                newName: "SoldQuantity");

            migrationBuilder.RenameIndex(
                name: "IX_Inventories_WarehouseId",
                table: "Inventories",
                newName: "IX_Inventories_WareHouseId");

            migrationBuilder.AlterColumn<int>(
                name: "WareHouseId",
                table: "Inventories",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Inventories_Warehouses_WareHouseId",
                table: "Inventories",
                column: "WareHouseId",
                principalTable: "Warehouses",
                principalColumn: "WarehouseId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Inventories_Warehouses_WareHouseId",
                table: "Inventories");

            migrationBuilder.RenameColumn(
                name: "WareHouseId",
                table: "Inventories",
                newName: "WarehouseId");

            migrationBuilder.RenameColumn(
                name: "SoldQuantity",
                table: "Inventories",
                newName: "ReservedQuantity");

            migrationBuilder.RenameIndex(
                name: "IX_Inventories_WareHouseId",
                table: "Inventories",
                newName: "IX_Inventories_WarehouseId");

            migrationBuilder.AlterColumn<int>(
                name: "WarehouseId",
                table: "Inventories",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Inventories_Warehouses_WarehouseId",
                table: "Inventories",
                column: "WarehouseId",
                principalTable: "Warehouses",
                principalColumn: "WarehouseId");
        }
    }
}
