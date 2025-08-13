using Microsoft.EntityFrameworkCore;

namespace Connect4.Web.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Jugador> Jugadores { get; set; }
        public DbSet<Partida> Partidas { get; set; }
        public DbSet<Movimiento> Movimientos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Relación doble jugador - partida
            modelBuilder.Entity<Partida>()
                .HasOne(p => p.Jugador1)
                .WithMany(j => j.PartidasComoJugador1)
                .HasForeignKey(p => p.Jugador1Id)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Partida>()
                .HasOne(p => p.Jugador2)
                .WithMany(j => j.PartidasComoJugador2)
                .HasForeignKey(p => p.Jugador2Id)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Partida>()
                .HasOne(p => p.TurnoJugador)
                .WithMany()
                .HasForeignKey(p => p.TurnoJugadorId)
                .OnDelete(DeleteBehavior.Restrict);

            // Para que EF Core que NO genere el ID automáticamente
            modelBuilder.Entity<Jugador>()
            .Property(j => j.JugadorId)
            .ValueGeneratedNever();

            // Seed jugadores
            modelBuilder.Entity<Jugador>().HasData(
                new Jugador { JugadorId = 1, Nombre = "Alice" },
                new Jugador { JugadorId = 2, Nombre = "Bob" }
            );

            // Seed partidas (depende de los jugadores)
            modelBuilder.Entity<Partida>().HasData(
                new Partida
                {
                    PartidaId = 1,
                    Jugador1Id = 1,
                    Jugador2Id = 2,
                    TurnoJugadorId = 1,
                    Estado = "EnCurso",
                    Resultado = "EnCurso",
                    FechaHora = new DateTime(2025, 8, 13, 12, 0, 0) // valor fijo
                }
            );   
            
        }
    }
}