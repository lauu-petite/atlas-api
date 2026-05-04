using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AtlasAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddSecurityAndRolesV1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Logros_Usuarios_UsuarioId",
                table: "Logros");

            migrationBuilder.RenameColumn(
                name: "Password",
                table: "Usuarios",
                newName: "Rol");

            migrationBuilder.AddColumn<bool>(
                name: "EstaBaneado",
                table: "Usuarios",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "PasswordHash",
                table: "Usuarios",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<int>(
                name: "UsuarioId",
                table: "Logros",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "CategoriaColor",
                table: "Eventos",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CategoriaIconoUrl",
                table: "Eventos",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "CategoriaNombre",
                table: "Eventos",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "MapaId",
                table: "Eventos",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Mapas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Nombre = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Descripcion = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    AnioReferencia = table.Column<int>(type: "int", nullable: false),
                    UrlGeoJson = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Leyenda = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Mapas", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "RespuestasPartida",
                columns: table => new
                {
                    PartidaId = table.Column<int>(type: "int", nullable: false),
                    PreguntaId = table.Column<int>(type: "int", nullable: false),
                    RespuestaSeleccionada = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    EsAcertada = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RespuestasPartida", x => new { x.PartidaId, x.PreguntaId });
                    table.ForeignKey(
                        name: "FK_RespuestasPartida_Partidas_PartidaId",
                        column: x => x.PartidaId,
                        principalTable: "Partidas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RespuestasPartida_Preguntas_PreguntaId",
                        column: x => x.PreguntaId,
                        principalTable: "Preguntas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "UsuarioEventoFavoritos",
                columns: table => new
                {
                    UsuarioId = table.Column<int>(type: "int", nullable: false),
                    EventoId = table.Column<int>(type: "int", nullable: false),
                    FechaGuardado = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsuarioEventoFavoritos", x => new { x.UsuarioId, x.EventoId });
                    table.ForeignKey(
                        name: "FK_UsuarioEventoFavoritos_Eventos_EventoId",
                        column: x => x.EventoId,
                        principalTable: "Eventos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UsuarioEventoFavoritos_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Eventos_MapaId",
                table: "Eventos",
                column: "MapaId");

            migrationBuilder.CreateIndex(
                name: "IX_RespuestasPartida_PreguntaId",
                table: "RespuestasPartida",
                column: "PreguntaId");

            migrationBuilder.CreateIndex(
                name: "IX_UsuarioEventoFavoritos_EventoId",
                table: "UsuarioEventoFavoritos",
                column: "EventoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Eventos_Mapas_MapaId",
                table: "Eventos",
                column: "MapaId",
                principalTable: "Mapas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Logros_Usuarios_UsuarioId",
                table: "Logros",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Eventos_Mapas_MapaId",
                table: "Eventos");

            migrationBuilder.DropForeignKey(
                name: "FK_Logros_Usuarios_UsuarioId",
                table: "Logros");

            migrationBuilder.DropTable(
                name: "Mapas");

            migrationBuilder.DropTable(
                name: "RespuestasPartida");

            migrationBuilder.DropTable(
                name: "UsuarioEventoFavoritos");

            migrationBuilder.DropIndex(
                name: "IX_Eventos_MapaId",
                table: "Eventos");

            migrationBuilder.DropColumn(
                name: "EstaBaneado",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "PasswordHash",
                table: "Usuarios");

            migrationBuilder.DropColumn(
                name: "CategoriaColor",
                table: "Eventos");

            migrationBuilder.DropColumn(
                name: "CategoriaIconoUrl",
                table: "Eventos");

            migrationBuilder.DropColumn(
                name: "CategoriaNombre",
                table: "Eventos");

            migrationBuilder.DropColumn(
                name: "MapaId",
                table: "Eventos");

            migrationBuilder.RenameColumn(
                name: "Rol",
                table: "Usuarios",
                newName: "Password");

            migrationBuilder.AlterColumn<int>(
                name: "UsuarioId",
                table: "Logros",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Logros_Usuarios_UsuarioId",
                table: "Logros",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
