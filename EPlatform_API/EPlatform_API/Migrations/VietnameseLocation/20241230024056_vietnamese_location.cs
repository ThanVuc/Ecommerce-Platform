using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EPlatform_API.Migrations.VietnameseLocation
{
    /// <inheritdoc />
    public partial class vietnamese_location : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AdministrativeRegions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CodeName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CodeNameEn = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdministrativeRegions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AdministrativeUnits",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    FullNameEn = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ShortName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ShortNameEn = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CodeName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CodeNameEn = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdministrativeUnits", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Provinces",
                columns: table => new
                {
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    FullName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    FullNameEn = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CodeName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    AdministrativeUnitId = table.Column<int>(type: "int", nullable: true),
                    AdministrativeRegionId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Provinces", x => x.Code);
                    table.ForeignKey(
                        name: "FK_Provinces_AdministrativeRegions_AdministrativeRegionId",
                        column: x => x.AdministrativeRegionId,
                        principalTable: "AdministrativeRegions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Provinces_AdministrativeUnits_AdministrativeUnitId",
                        column: x => x.AdministrativeUnitId,
                        principalTable: "AdministrativeUnits",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Districts",
                columns: table => new
                {
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    FullName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    FullNameEn = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CodeName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    ProvinceCode = table.Column<string>(type: "nvarchar(20)", nullable: true),
                    AdministrativeUnitId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Districts", x => x.Code);
                    table.ForeignKey(
                        name: "FK_Districts_AdministrativeUnits_AdministrativeUnitId",
                        column: x => x.AdministrativeUnitId,
                        principalTable: "AdministrativeUnits",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Districts_Provinces_ProvinceCode",
                        column: x => x.ProvinceCode,
                        principalTable: "Provinces",
                        principalColumn: "Code");
                });

            migrationBuilder.CreateTable(
                name: "Wards",
                columns: table => new
                {
                    Code = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    FullName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    FullNameEn = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CodeName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    DistrictCode = table.Column<string>(type: "nvarchar(20)", nullable: true),
                    AdministrativeUnitId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Wards", x => x.Code);
                    table.ForeignKey(
                        name: "FK_Wards_AdministrativeUnits_AdministrativeUnitId",
                        column: x => x.AdministrativeUnitId,
                        principalTable: "AdministrativeUnits",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Wards_Districts_DistrictCode",
                        column: x => x.DistrictCode,
                        principalTable: "Districts",
                        principalColumn: "Code");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Districts_AdministrativeUnitId",
                table: "Districts",
                column: "AdministrativeUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_Districts_ProvinceCode",
                table: "Districts",
                column: "ProvinceCode");

            migrationBuilder.CreateIndex(
                name: "IX_Provinces_AdministrativeRegionId",
                table: "Provinces",
                column: "AdministrativeRegionId");

            migrationBuilder.CreateIndex(
                name: "IX_Provinces_AdministrativeUnitId",
                table: "Provinces",
                column: "AdministrativeUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_Wards_AdministrativeUnitId",
                table: "Wards",
                column: "AdministrativeUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_Wards_DistrictCode",
                table: "Wards",
                column: "DistrictCode");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Wards");

            migrationBuilder.DropTable(
                name: "Districts");

            migrationBuilder.DropTable(
                name: "Provinces");

            migrationBuilder.DropTable(
                name: "AdministrativeRegions");

            migrationBuilder.DropTable(
                name: "AdministrativeUnits");
        }
    }
}
