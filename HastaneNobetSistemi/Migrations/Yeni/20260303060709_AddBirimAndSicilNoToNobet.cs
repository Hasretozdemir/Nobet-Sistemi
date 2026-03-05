using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HastaneNobetSistemi.Migrations.Yeni
{
    /// <inheritdoc />
    public partial class AddBirimAndSicilNoToNobet : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NobetTakaslar_Nobetler_KarsilikNobetId",
                table: "NobetTakaslar");

            migrationBuilder.DropForeignKey(
                name: "FK_NobetTakaslar_Nobetler_TeklifEdilenNobetId",
                table: "NobetTakaslar");

            migrationBuilder.DropForeignKey(
                name: "FK_NobetTakaslar_Personeller_HedefPersonelId",
                table: "NobetTakaslar");

            migrationBuilder.DropForeignKey(
                name: "FK_NobetTakaslar_Personeller_TeklifEdenPersonelId",
                table: "NobetTakaslar");

            migrationBuilder.AlterColumn<bool>(
                name: "YedekMi",
                table: "Personeller",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Birim",
                table: "Personeller",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SicilNo",
                table: "Personeller",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_NobetTakaslar_Nobetler_KarsilikNobetId",
                table: "NobetTakaslar",
                column: "KarsilikNobetId",
                principalTable: "Nobetler",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_NobetTakaslar_Nobetler_TeklifEdilenNobetId",
                table: "NobetTakaslar",
                column: "TeklifEdilenNobetId",
                principalTable: "Nobetler",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_NobetTakaslar_Personeller_HedefPersonelId",
                table: "NobetTakaslar",
                column: "HedefPersonelId",
                principalTable: "Personeller",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_NobetTakaslar_Personeller_TeklifEdenPersonelId",
                table: "NobetTakaslar",
                column: "TeklifEdenPersonelId",
                principalTable: "Personeller",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NobetTakaslar_Nobetler_KarsilikNobetId",
                table: "NobetTakaslar");

            migrationBuilder.DropForeignKey(
                name: "FK_NobetTakaslar_Nobetler_TeklifEdilenNobetId",
                table: "NobetTakaslar");

            migrationBuilder.DropForeignKey(
                name: "FK_NobetTakaslar_Personeller_HedefPersonelId",
                table: "NobetTakaslar");

            migrationBuilder.DropForeignKey(
                name: "FK_NobetTakaslar_Personeller_TeklifEdenPersonelId",
                table: "NobetTakaslar");

            migrationBuilder.DropColumn(
                name: "Birim",
                table: "Personeller");

            migrationBuilder.DropColumn(
                name: "SicilNo",
                table: "Personeller");

            migrationBuilder.AlterColumn<bool>(
                name: "YedekMi",
                table: "Personeller",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit");

            migrationBuilder.AddForeignKey(
                name: "FK_NobetTakaslar_Nobetler_KarsilikNobetId",
                table: "NobetTakaslar",
                column: "KarsilikNobetId",
                principalTable: "Nobetler",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_NobetTakaslar_Nobetler_TeklifEdilenNobetId",
                table: "NobetTakaslar",
                column: "TeklifEdilenNobetId",
                principalTable: "Nobetler",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_NobetTakaslar_Personeller_HedefPersonelId",
                table: "NobetTakaslar",
                column: "HedefPersonelId",
                principalTable: "Personeller",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_NobetTakaslar_Personeller_TeklifEdenPersonelId",
                table: "NobetTakaslar",
                column: "TeklifEdenPersonelId",
                principalTable: "Personeller",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
