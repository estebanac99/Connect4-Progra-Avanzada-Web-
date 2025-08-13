using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Connect4.Web.Migrations
{
    /// <inheritdoc />
    public partial class UpdateJugadorIdManual : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //  Eliminar FK de Partidas
        migrationBuilder.DropForeignKey(name: "FK_Partidas_Jugadores_Jugador1Id", table: "Partidas");
        migrationBuilder.DropForeignKey(name: "FK_Partidas_Jugadores_Jugador2Id", table: "Partidas");
        migrationBuilder.DropForeignKey(name: "FK_Partidas_Jugadores_TurnoJugadorId", table: "Partidas");

        //  Eliminar FK de Movimientos
        migrationBuilder.DropForeignKey(name: "FK_Movimientos_Jugadores_JugadorId", table: "Movimientos");

        //  Borrar la tabla Jugadores
        migrationBuilder.DropTable(name: "Jugadores");

        //  Crear la tabla Jugadores sin IDENTITY
        migrationBuilder.CreateTable(
            name: "Jugadores",
            columns: table => new
            {
                JugadorId = table.Column<int>(nullable: false), // ahora manual
                Nombre = table.Column<string>(nullable: false, defaultValue: ""),
                Marcador = table.Column<int>(nullable: false, defaultValue: 0),
                Ganadas = table.Column<int>(nullable: false, defaultValue: 0),
                Perdidas = table.Column<int>(nullable: false, defaultValue: 0),
                Empatadas = table.Column<int>(nullable: false, defaultValue: 0)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Jugadores", x => x.JugadorId);
            });

        //  Volver a crear FK de Partidas
        migrationBuilder.AddForeignKey(
            name: "FK_Partidas_Jugadores_Jugador1Id",
            table: "Partidas",
            column: "Jugador1Id",
            principalTable: "Jugadores",
            principalColumn: "JugadorId",
            onDelete: ReferentialAction.Restrict);

        migrationBuilder.AddForeignKey(
            name: "FK_Partidas_Jugadores_Jugador2Id",
            table: "Partidas",
            column: "Jugador2Id",
            principalTable: "Jugadores",
            principalColumn: "JugadorId",
            onDelete: ReferentialAction.Restrict);

        migrationBuilder.AddForeignKey(
            name: "FK_Partidas_Jugadores_TurnoJugadorId",
            table: "Partidas",
            column: "TurnoJugadorId",
            principalTable: "Jugadores",
            principalColumn: "JugadorId",
            onDelete: ReferentialAction.Restrict);

        //  Volver a crear FK de Movimientos
        migrationBuilder.AddForeignKey(
            name: "FK_Movimientos_Jugadores_JugadorId",
            table: "Movimientos",
            column: "JugadorId",
            principalTable: "Jugadores",
            principalColumn: "JugadorId",
            onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Revertir: eliminar FKs
        migrationBuilder.DropForeignKey(name: "FK_Partidas_Jugadores_Jugador1Id", table: "Partidas");
        migrationBuilder.DropForeignKey(name: "FK_Partidas_Jugadores_Jugador2Id", table: "Partidas");
        migrationBuilder.DropForeignKey(name: "FK_Partidas_Jugadores_TurnoJugadorId", table: "Partidas");
        migrationBuilder.DropForeignKey(name: "FK_Movimientos_Jugadores_JugadorId", table: "Movimientos");

        // Borrar la tabla Jugadores actual
        migrationBuilder.DropTable(name: "Jugadores");

        // Recrear la tabla con IDENTITY como estaba antes
        migrationBuilder.CreateTable(
            name: "Jugadores",
            columns: table => new
            {
                JugadorId = table.Column<int>(nullable: false)
                    .Annotation("SqlServer:Identity", "1, 1"), // volvemos a identity
                Nombre = table.Column<string>(nullable: false, defaultValue: ""),
                Marcador = table.Column<int>(nullable: false, defaultValue: 0),
                Ganadas = table.Column<int>(nullable: false, defaultValue: 0),
                Perdidas = table.Column<int>(nullable: false, defaultValue: 0),
                Empatadas = table.Column<int>(nullable: false, defaultValue: 0)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Jugadores", x => x.JugadorId);
            });

        // Volver a recrear las FK originales
        migrationBuilder.AddForeignKey(
            name: "FK_Partidas_Jugadores_Jugador1Id",
            table: "Partidas",
            column: "Jugador1Id",
            principalTable: "Jugadores",
            principalColumn: "JugadorId",
            onDelete: ReferentialAction.Restrict);

        migrationBuilder.AddForeignKey(
            name: "FK_Partidas_Jugadores_Jugador2Id",
            table: "Partidas",
            column: "Jugador2Id",
            principalTable: "Jugadores",
            principalColumn: "JugadorId",
            onDelete: ReferentialAction.Restrict);

        migrationBuilder.AddForeignKey(
            name: "FK_Partidas_Jugadores_TurnoJugadorId",
            table: "Partidas",
            column: "TurnoJugadorId",
            principalTable: "Jugadores",
            principalColumn: "JugadorId",
            onDelete: ReferentialAction.Restrict);

        migrationBuilder.AddForeignKey(
            name: "FK_Movimientos_Jugadores_JugadorId",
            table: "Movimientos",
            column: "JugadorId",
            principalTable: "Jugadores",
            principalColumn: "JugadorId",
            onDelete: ReferentialAction.Restrict);
        }
    }
}
