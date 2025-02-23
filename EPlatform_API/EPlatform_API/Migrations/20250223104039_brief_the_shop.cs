using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EPlatform_API.Migrations
{
    /// <inheritdoc />
    public partial class brief_the_shop : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InvoiceEmail",
                table: "Shops");

            migrationBuilder.DropColumn(
                name: "PickUpAddress",
                table: "Shops");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "InvoiceEmail",
                table: "Shops",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PickUpAddress",
                table: "Shops",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "");
        }
    }
}
