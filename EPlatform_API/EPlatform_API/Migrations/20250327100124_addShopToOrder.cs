using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EPlatform_API.Migrations
{
    /// <inheritdoc />
    public partial class addShopToOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ShopId",
                table: "Orders",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_ShopId",
                table: "Orders",
                column: "ShopId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Shops_ShopId",
                table: "Orders",
                column: "ShopId",
                principalTable: "Shops",
                principalColumn: "ShopId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Shops_ShopId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_ShopId",
                table: "Orders");

            migrationBuilder.AlterColumn<string>(
                name: "ShopId",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
