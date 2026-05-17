using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AtlasAPI.Migrations
{
    /// <inheritdoc />
    public partial class RemoveTipoYPeriodoDeEventos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Tipo",
                table: "Eventos");

            migrationBuilder.DropColumn(
                name: "Periodo",
                table: "Eventos");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Tipo",
                table: "Eventos",
                type: "longtext",
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "Periodo",
                table: "Eventos",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
