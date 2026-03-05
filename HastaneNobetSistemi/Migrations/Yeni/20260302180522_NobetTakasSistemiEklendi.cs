using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HastaneNobetSistemi.Migrations.Yeni
{
    /// <inheritdoc />
    public partial class NobetTakasSistemiEklendi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Aciklama",
                table: "NobetYayinlari");

            migrationBuilder.DropColumn(
                name: "YayinlayanKullanici",
                table: "NobetYayinlari");

            migrationBuilder.RenameColumn(
                name: "YayinTarihi",
                table: "NobetYayinlari",
                newName: "YayinlanmaTarihi");

            migrationBuilder.RenameColumn(
                name: "AktifMi",
                table: "NobetYayinlari",
                newName: "YayindaMi");

            migrationBuilder.CreateTable(
                name: "NobetTakaslar",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TeklifEdenPersonelId = table.Column<int>(type: "int", nullable: false),
                    TeklifEdilenNobetId = table.Column<int>(type: "int", nullable: false),
                    HedefPersonelId = table.Column<int>(type: "int", nullable: true),
                    KarsilikNobetId = table.Column<int>(type: "int", nullable: true),
                    Aciklama = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Durum = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    OlusturmaTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    YanitTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RedNedeni = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NobetTakaslar", x => x.Id);

                    // ✅ TÜM FOREIGN KEY'LER NO ACTION OLMALI
                    table.ForeignKey(
                        name: "FK_NobetTakaslar_Personeller_TeklifEdenPersonelId",
                        column: x => x.TeklifEdenPersonelId,
                        principalTable: "Personeller",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);

                    table.ForeignKey(
                        name: "FK_NobetTakaslar_Nobetler_TeklifEdilenNobetId",
                        column: x => x.TeklifEdilenNobetId,
                        principalTable: "Nobetler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);

                    table.ForeignKey(
                        name: "FK_NobetTakaslar_Personeller_HedefPersonelId",
                        column: x => x.HedefPersonelId,
                        principalTable: "Personeller",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction); // ✅ SetNull → NoAction

                    table.ForeignKey(
                        name: "FK_NobetTakaslar_Nobetler_KarsilikNobetId",
                        column: x => x.KarsilikNobetId,
                        principalTable: "Nobetler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction); // ✅ SetNull → NoAction
                });

            migrationBuilder.CreateIndex(
                name: "IX_NobetTakaslar_HedefPersonelId",
                table: "NobetTakaslar",
                column: "HedefPersonelId");

            migrationBuilder.CreateIndex(
                name: "IX_NobetTakaslar_KarsilikNobetId",
                table: "NobetTakaslar",
                column: "KarsilikNobetId");

            migrationBuilder.CreateIndex(
                name: "IX_NobetTakaslar_TeklifEdenPersonelId",
                table: "NobetTakaslar",
                column: "TeklifEdenPersonelId");

            migrationBuilder.CreateIndex(
                name: "IX_NobetTakaslar_TeklifEdilenNobetId",
                table: "NobetTakaslar",
                column: "TeklifEdilenNobetId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NobetTakaslar");

            migrationBuilder.RenameColumn(
                name: "YayinlanmaTarihi",
                table: "NobetYayinlari",
                newName: "YayinTarihi");

            migrationBuilder.RenameColumn(
                name: "YayindaMi",
                table: "NobetYayinlari",
                newName: "AktifMi");

            migrationBuilder.AddColumn<string>(
                name: "Aciklama",
                table: "NobetYayinlari",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "YayinlayanKullanici",
                table: "NobetYayinlari",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}