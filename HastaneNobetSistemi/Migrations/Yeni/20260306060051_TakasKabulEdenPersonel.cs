using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HastaneNobetSistemi.Migrations.Yeni
{
    /// <inheritdoc />
    public partial class TakasKabulEdenPersonel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "KabulEdenPersonelId",
                table: "NobetTakaslar",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_NobetTakaslar_KabulEdenPersonelId",
                table: "NobetTakaslar",
                column: "KabulEdenPersonelId");

            migrationBuilder.AddForeignKey(
                name: "FK_NobetTakaslar_Personeller_KabulEdenPersonelId",
                table: "NobetTakaslar",
                column: "KabulEdenPersonelId",
                principalTable: "Personeller",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NobetTakaslar_Personeller_KabulEdenPersonelId",
                table: "NobetTakaslar");

            migrationBuilder.DropIndex(
                name: "IX_NobetTakaslar_KabulEdenPersonelId",
                table: "NobetTakaslar");

            migrationBuilder.DropColumn(
                name: "KabulEdenPersonelId",
                table: "NobetTakaslar");
        }
    }
}
