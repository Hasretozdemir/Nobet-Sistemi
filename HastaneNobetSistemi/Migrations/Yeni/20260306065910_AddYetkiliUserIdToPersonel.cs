using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HastaneNobetSistemi.Migrations.Yeni
{
    /// <inheritdoc />
    public partial class AddYetkiliUserIdToPersonel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "YetkiliUserId",
                table: "Personeller",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Personeller_YetkiliUserId",
                table: "Personeller",
                column: "YetkiliUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Personeller_AspNetUsers_YetkiliUserId",
                table: "Personeller",
                column: "YetkiliUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Personeller_AspNetUsers_YetkiliUserId",
                table: "Personeller");

            migrationBuilder.DropIndex(
                name: "IX_Personeller_YetkiliUserId",
                table: "Personeller");

            migrationBuilder.DropColumn(
                name: "YetkiliUserId",
                table: "Personeller");
        }
    }
}
