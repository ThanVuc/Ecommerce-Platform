using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EPlatform_API.Migrations
{
    /// <inheritdoc />
    public partial class adjustTimeOfOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeliveryDate",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "TaxAmount",
                table: "Orders");

            migrationBuilder.RenameColumn(
                name: "ShippingDate",
                table: "Orders",
                newName: "ExpectedDeliveryDate");

            migrationBuilder.RenameColumn(
                name: "OrderDate",
                table: "Orders",
                newName: "CompleteOrderDate");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ExpectedDeliveryDate",
                table: "Orders",
                newName: "ShippingDate");

            migrationBuilder.RenameColumn(
                name: "CompleteOrderDate",
                table: "Orders",
                newName: "OrderDate");

            migrationBuilder.AddColumn<DateTime>(
                name: "DeliveryDate",
                table: "Orders",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TaxAmount",
                table: "Orders",
                type: "decimal(18,2)",
                nullable: true);
        }
    }
}
