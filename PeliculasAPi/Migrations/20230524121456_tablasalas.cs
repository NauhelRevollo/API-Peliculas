using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PeliculasAPi.Migrations
{
    public partial class tablasalas : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PeliculasSalasDeCine_SalaDeCine_SalaDeCineId",
                table: "PeliculasSalasDeCine");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SalaDeCine",
                table: "SalaDeCine");

            migrationBuilder.RenameTable(
                name: "SalaDeCine",
                newName: "SalasDeCine");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SalasDeCine",
                table: "SalasDeCine",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PeliculasSalasDeCine_SalasDeCine_SalaDeCineId",
                table: "PeliculasSalasDeCine",
                column: "SalaDeCineId",
                principalTable: "SalasDeCine",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PeliculasSalasDeCine_SalasDeCine_SalaDeCineId",
                table: "PeliculasSalasDeCine");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SalasDeCine",
                table: "SalasDeCine");

            migrationBuilder.RenameTable(
                name: "SalasDeCine",
                newName: "SalaDeCine");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SalaDeCine",
                table: "SalaDeCine",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PeliculasSalasDeCine_SalaDeCine_SalaDeCineId",
                table: "PeliculasSalasDeCine",
                column: "SalaDeCineId",
                principalTable: "SalaDeCine",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
