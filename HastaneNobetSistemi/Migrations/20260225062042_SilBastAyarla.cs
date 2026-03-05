using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HastaneNobetSistemi.Migrations
{
    /// <inheritdoc />
    public partial class SilBastAyarla : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Bayramlar",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BayramAdi = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    BaslangicTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BitisTarihi = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bayramlar", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Personeller",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AdSoyad = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AktifMi = table.Column<bool>(type: "bit", nullable: true),
                    HaftaIciSira = table.Column<int>(type: "int", nullable: false),
                    HaftaSonuSira = table.Column<int>(type: "int", nullable: false),
                    BayramSira = table.Column<int>(type: "int", nullable: false),
                    BuAyHaftaIci = table.Column<int>(type: "int", nullable: false),
                    BuAyHaftaSonu = table.Column<int>(type: "int", nullable: false),
                    BuAyBayram = table.Column<int>(type: "int", nullable: false),
                    SonNobetTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IseGirisTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ToplamHaftaIci = table.Column<int>(type: "int", nullable: false),
                    ToplamHaftaSonu = table.Column<int>(type: "int", nullable: false),
                    ToplamBayram = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Personeller", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Nobetler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PersonelId = table.Column<int>(type: "int", nullable: false),
                    Tarih = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PuanDegeri = table.Column<int>(type: "int", nullable: false),
                    NobetTipi = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Nobetler", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Nobetler_Personeller_PersonelId",
                        column: x => x.PersonelId,
                        principalTable: "Personeller",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Nobetler_PersonelId",
                table: "Nobetler",
                column: "PersonelId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Bayramlar");

            migrationBuilder.DropTable(
                name: "Nobetler");

            migrationBuilder.DropTable(
                name: "Personeller");
        }
    }
}
