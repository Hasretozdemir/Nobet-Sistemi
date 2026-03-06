using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HastaneNobetSistemi.Migrations.Yeni
{
    /// <inheritdoc />
    public partial class NobetUcretSistemi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NobetTakaslar_Nobetler_TeklifEdilenNobetId",
                table: "NobetTakaslar");

            migrationBuilder.AddColumn<decimal>(
                name: "NobetUcreti",
                table: "Personeller",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "Unvan",
                table: "Personeller",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_NobetTakaslar_Nobetler_TeklifEdilenNobetId",
                table: "NobetTakaslar",
                column: "TeklifEdilenNobetId",
                principalTable: "Nobetler",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NobetTakaslar_Nobetler_TeklifEdilenNobetId",
                table: "NobetTakaslar");

            migrationBuilder.DropColumn(
                name: "NobetUcreti",
                table: "Personeller");

            migrationBuilder.DropColumn(
                name: "Unvan",
                table: "Personeller");

            migrationBuilder.AddForeignKey(
                name: "FK_NobetTakaslar_Nobetler_TeklifEdilenNobetId",
                table: "NobetTakaslar",
                column: "TeklifEdilenNobetId",
                principalTable: "Nobetler",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
