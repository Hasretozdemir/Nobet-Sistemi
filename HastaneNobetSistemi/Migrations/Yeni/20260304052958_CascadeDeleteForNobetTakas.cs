using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HastaneNobetSistemi.Migrations.Yeni
{
    /// <inheritdoc />
    public partial class CascadeDeleteForNobetTakas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NobetTakaslar_Nobetler_TeklifEdilenNobetId",
                table: "NobetTakaslar");

            migrationBuilder.AddForeignKey(
                name: "FK_NobetTakaslar_Nobetler_TeklifEdilenNobetId",
                table: "NobetTakaslar",
                column: "TeklifEdilenNobetId",
                principalTable: "Nobetler",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NobetTakaslar_Nobetler_TeklifEdilenNobetId",
                table: "NobetTakaslar");

            migrationBuilder.AddForeignKey(
                name: "FK_NobetTakaslar_Nobetler_TeklifEdilenNobetId",
                table: "NobetTakaslar",
                column: "TeklifEdilenNobetId",
                principalTable: "Nobetler",
                principalColumn: "Id");
        }
    }
}
