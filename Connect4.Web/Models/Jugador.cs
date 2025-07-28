using System.Collections.Generic;

public class Jugador
{
    public int JugadorId { get; set; }  // Identificación única
    public string Nombre { get; set; } = string.Empty;

    // Estadísticas
    public int Marcador { get; set; } = 0;
    public int Ganadas { get; set; } = 0;
    public int Perdidas { get; set; } = 0;
    public int Empatadas { get; set; } = 0;

    // Relaciones
    public ICollection<Partida> PartidasComoJugador1 { get; set; } = new List<Partida>();
    public ICollection<Partida> PartidasComoJugador2 { get; set; } = new List<Partida>();
    public ICollection<Movimiento> Movimientos { get; set; } = new List<Movimiento>();
}