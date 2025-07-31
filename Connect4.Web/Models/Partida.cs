using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

public class Partida
{
    [Display(Name = "ID")]
    public int PartidaId { get; set; }

    [Display(Name = "Fecha y Hora")]
    public DateTime FechaHora { get; set; } = DateTime.Now;

    
    public int Jugador1Id { get; set; }
    public int Jugador2Id { get; set; }

    public string Estado { get; set; } = "En Curso"; // O Finalizada
    public int TurnoJugadorId { get; set; }
    public string Resultado { get; set; } = "En Curso"; // VictoriaJugador1, VictoriaJugador2, Empate

    // Relaciones
    public Jugador? Jugador1 { get; set; }
    public Jugador? Jugador2 { get; set; }
    public Jugador? TurnoJugador { get; set; }

    public ICollection<Movimiento> Movimientos { get; set; } = new List<Movimiento>();
}