using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HastaneNobetSistemi.Migrations.Yeni
{
    /// <inheritdoc />
    public partial class YedekMiPropertyEklendi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "YedekMi",
                table: "Personeller",
                type: "bit",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "YedekMi",
                table: "Personeller");
        }
    }
}
