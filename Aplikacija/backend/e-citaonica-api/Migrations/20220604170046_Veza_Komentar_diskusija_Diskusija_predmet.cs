using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace e_citaonica_api.Migrations
{
    public partial class Veza_Komentar_diskusija_Diskusija_predmet : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DiskusijaId",
                table: "Objave",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PredmetId",
                table: "Objave",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Objave_DiskusijaId",
                table: "Objave",
                column: "DiskusijaId");

            migrationBuilder.CreateIndex(
                name: "IX_Objave_PredmetId",
                table: "Objave",
                column: "PredmetId");

            migrationBuilder.AddForeignKey(
                name: "FK_Objave_Objave_DiskusijaId",
                table: "Objave",
                column: "DiskusijaId",
                principalTable: "Objave",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Objave_Predmeti_PredmetId",
                table: "Objave",
                column: "PredmetId",
                principalTable: "Predmeti",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Objave_Objave_DiskusijaId",
                table: "Objave");

            migrationBuilder.DropForeignKey(
                name: "FK_Objave_Predmeti_PredmetId",
                table: "Objave");

            migrationBuilder.DropIndex(
                name: "IX_Objave_DiskusijaId",
                table: "Objave");

            migrationBuilder.DropIndex(
                name: "IX_Objave_PredmetId",
                table: "Objave");

            migrationBuilder.DropColumn(
                name: "DiskusijaId",
                table: "Objave");

            migrationBuilder.DropColumn(
                name: "PredmetId",
                table: "Objave");
        }
    }
}
