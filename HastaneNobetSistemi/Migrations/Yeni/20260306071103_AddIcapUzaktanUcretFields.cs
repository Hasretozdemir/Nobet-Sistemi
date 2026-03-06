using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HastaneNobetSistemi.Migrations.Yeni
{
    /// <inheritdoc />
    public partial class AddIcapUzaktanUcretFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "IcapSaat",
                table: "Personeller",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "IcapSaatlikUcret",
                table: "Personeller",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "UcretTipi",
                table: "Personeller",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "UzaktanSaat",
                table: "Personeller",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "UzaktanSaatlikUcret",
                table: "Personeller",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IcapSaat",
                table: "Personeller");

            migrationBuilder.DropColumn(
                name: "IcapSaatlikUcret",
                table: "Personeller");

            migrationBuilder.DropColumn(
                name: "UcretTipi",
                table: "Personeller");

            migrationBuilder.DropColumn(
                name: "UzaktanSaat",
                table: "Personeller");

            migrationBuilder.DropColumn(
                name: "UzaktanSaatlikUcret",
                table: "Personeller");
        }
    }
}
