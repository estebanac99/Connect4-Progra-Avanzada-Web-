using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

public class Partida
{
    [Display(Name = "ID")]
    public int PartidaId { get; set; }

    [Display(Name = "Fecha y Hora")]
    public DateTime FechaHora { get; set; } = DateTime.Now;

    [Display(Name = "Jugador 1")]
    public int Jugador1Id { get; set; }
    [Display(Name = "Jugador 2")]
    public int Jugador2Id { get; set; }

    public string Estado { get; set; } = "EnCurso"; // O Finalizada
    [Display(Name = "Turno Inicial")]
    public int TurnoJugadorId { get; set; }
    public string Resultado { get; set; } = "EnCurso"; // VictoriaJugador1, VictoriaJugador2, Empate

    

    // Relaciones
    public Jugador? Jugador1 { get; set; }
    public Jugador? Jugador2 { get; set; }
    public Jugador? TurnoJugador { get; set; }

    public ICollection<Movimiento> Movimientos { get; set; } = new List<Movimiento>();
    
    // NUEVAS PROPIEDADES LEÍBLES
    public string EstadoLegible
    {
        get
        {
            return Estado switch
            {
                "EnCurso" => "En Curso",
                "Finalizada" => "Finalizada",
                _ => Estado
            };
        }
    }

    public string ResultadoLegible
    {
        get
        {
            return Resultado switch
            {
                "EnCurso" => "En Curso",
                "Empate" => "Empate",
                "VictoriaJugador1" => Jugador1 != null ? $"Victoria de {Jugador1.Nombre}" : "Victoria del Jugador 1",
                "VictoriaJugador2" => Jugador2 != null ? $"Victoria de {Jugador2.Nombre}" : "Victoria del Jugador 2",
                _ => Resultado
            };
        }
    }
}