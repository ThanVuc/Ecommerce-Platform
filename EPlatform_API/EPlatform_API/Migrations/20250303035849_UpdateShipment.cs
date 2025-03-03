using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EPlatform_API.Migrations
{
    /// <inheritdoc />
    public partial class UpdateShipment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Shipments_ShipmentCarrier_CarrierId",
                table: "Shipments");

            migrationBuilder.AlterColumn<int>(
                name: "CarrierId",
                table: "Shipments",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Shipments_ShipmentCarrier_CarrierId",
                table: "Shipments",
                column: "CarrierId",
                principalTable: "ShipmentCarrier",
                principalColumn: "CarrierId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Shipments_ShipmentCarrier_CarrierId",
                table: "Shipments");

            migrationBuilder.AlterColumn<int>(
                name: "CarrierId",
                table: "Shipments",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Shipments_ShipmentCarrier_CarrierId",
                table: "Shipments",
                column: "CarrierId",
                principalTable: "ShipmentCarrier",
                principalColumn: "CarrierId");
        }
    }
}
