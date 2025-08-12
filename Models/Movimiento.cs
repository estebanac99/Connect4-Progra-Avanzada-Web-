public class Movimiento
{
    public int MovimientoId { get; set; }

    public int PartidaId { get; set; }
    public int JugadorId { get; set; }

    public char Columna { get; set; } // 'A' a 'G'
    public int Fila { get; set; }     // 0 (inferior) a 5 (superior)
    public int OrdenTurno { get; set; }

    public DateTime FechaHora { get; set; } = DateTime.Now;

    // Relaciones
    public Partida? Partida { get; set; }
    public Jugador? Jugador { get; set; }
}