using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HastaneNobetSistemi.Migrations.Yeni
{
    /// <inheritdoc />
    public partial class GelismisAdaletSistemi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "AdSoyad",
                table: "Personeller",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<DateTime>(
                name: "IzinBaslangic",
                table: "Personeller",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "IzinBitis",
                table: "Personeller",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "ZorunluNobetciMi",
                table: "Personeller",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IzinBaslangic",
                table: "Personeller");

            migrationBuilder.DropColumn(
                name: "IzinBitis",
                table: "Personeller");

            migrationBuilder.DropColumn(
                name: "ZorunluNobetciMi",
                table: "Personeller");

            migrationBuilder.AlterColumn<string>(
                name: "AdSoyad",
                table: "Personeller",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);
        }
    }
}
