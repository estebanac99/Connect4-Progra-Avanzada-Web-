using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Connect4.Web.Models;
using System.Text.Json;

namespace Connect4.Web.Controllers
{
    public class PartidaController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PartidaController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Partida
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Partidas.Include(p => p.Jugador1).Include(p => p.Jugador2).Include(p => p.TurnoJugador);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Partida/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var partida = await _context.Partidas
                .Include(p => p.Jugador1)
                .Include(p => p.Jugador2)
                .Include(p => p.TurnoJugador)
                .FirstOrDefaultAsync(m => m.PartidaId == id);
            if (partida == null)
            {
                return NotFound();
            }

            return View(partida);
        }

        // GET: Partida/Create
        public IActionResult Create()
        {
            var jugadores = new SelectList(_context.Jugadores, "JugadorId", "Nombre");
            ViewData["Jugador1Id"] = jugadores;
            ViewData["Jugador2Id"] = jugadores;
            return View();
        }

        // POST: Partida/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Jugador1Id,Jugador2Id")] Partida partida)
        {
            // Validación: los jugadores no pueden ser el mismo
            if (partida.Jugador1Id == partida.Jugador2Id)
            {
                ModelState.AddModelError(string.Empty, "Los jugadores deben ser distintos.");
            }

            if (ModelState.IsValid)
            {
                // Asignación automática
                partida.FechaHora = DateTime.Now;
                partida.Estado = "EnCurso";
                partida.Resultado = "EnCurso";
                partida.TurnoJugadorId = partida.Jugador1Id; // Jugador 1 siempre inicia

                _context.Add(partida);
                await _context.SaveChangesAsync();
                return RedirectToAction("Tablero", new { id = partida.PartidaId });
            }

            // Recargar dropdowns si hay error
            var jugadores = new SelectList(_context.Jugadores, "JugadorId", "Nombre");
            ViewData["Jugador1Id"] = jugadores;
            ViewData["Jugador2Id"] = jugadores;

            return View(partida);
        }

        // GET: Partida/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var partida = await _context.Partidas
                .Include(p => p.Jugador1)
                .Include(p => p.Jugador2)
                .Include(p => p.TurnoJugador)
                .FirstOrDefaultAsync(m => m.PartidaId == id);
            if (partida == null)
            {
                return NotFound();
            }

            return View(partida);
        }

        // POST: Partida/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var partida = await _context.Partidas.FindAsync(id);
            if (partida != null)
            {
                _context.Partidas.Remove(partida);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Tablero(int id)
        {
            var partida = await _context.Partidas
                .Include(p => p.Movimientos)
                .Include(p => p.Jugador1)
                .Include(p => p.Jugador2)
                .Include(p => p.TurnoJugador)
                .FirstOrDefaultAsync(p => p.PartidaId == id);

            if (partida == null)
                return NotFound();

            // Crear tablero 6x7 vacío
            char[,] tablero = new char[6, 7];
            for (int fila = 0; fila < 6; fila++)
            {
                for (int col = 0; col < 7; col++)
                {
                    tablero[fila, col] = ' ';
                }
            }

            foreach (var mov in partida.Movimientos)
            {
                int colIdx = mov.Columna - 'A';
                tablero[mov.Fila, colIdx] = mov.JugadorId == partida.Jugador1Id ? 'X' : 'O';
            }

            ViewBag.Tablero = tablero;
            ViewBag.Partida = partida;

            // ✅ Convertir coordenadas ganadoras (si existen) en HashSet
            if (TempData["CoordenadasGanadoras"] is string json)
            {
                var claves = JsonSerializer.Deserialize<List<string>>(json);
                ViewBag.CoordenadasGanadoras = new HashSet<string>(claves!);
            }
            else
            {
                ViewBag.CoordenadasGanadoras = new HashSet<string>();
            }

            return View();
        }

        // POST: Partida/Jugar/5
        [HttpPost]
        public async Task<IActionResult> Jugar(int id, char columna)
        {
            // Buscar partida con jugadores y movimientos
            var partida = await _context.Partidas
            .Include(p => p.Movimientos)
            .Include(p => p.Jugador1)
            .Include(p => p.Jugador2)
            .Include(p => p.TurnoJugador)
            .FirstOrDefaultAsync(p => p.PartidaId == id);

            // Verificar si existe y no está finalizada
            if (partida == null || partida.Estado == "Finalizada")
            {
                TempData["Error"] = "La partida ya ha finalizado.";
                return RedirectToAction(nameof(Tablero), new { id });
            }

            if (columna < 'A' || columna > 'G')
            {
                TempData["Error"] = "Columna inválida.";
                return RedirectToAction(nameof(Tablero), new { id });
            }

            var movimientosColumna = partida.Movimientos
                .Where(m => m.Columna == columna)
                .OrderBy(m => m.Fila)
                .ToList();

            if (movimientosColumna.Count >= 6)
            {
                TempData["Error"] = "La columna está llena.";
                return RedirectToAction(nameof(Tablero), new { id });
            }

            int fila = movimientosColumna.Count;
            int nuevoOrden = partida.Movimientos.Count > 0
                ? partida.Movimientos.Max(m => m.OrdenTurno) + 1
                : 1;

            var nuevoMovimiento = new Movimiento
            {
                PartidaId = id,
                JugadorId = partida.TurnoJugadorId,
                Columna = columna,
                Fila = fila,
                OrdenTurno = nuevoOrden,
                FechaHora = DateTime.Now
            };

            _context.Movimientos.Add(nuevoMovimiento);
            await _context.SaveChangesAsync();

            // Volver a cargar los movimientos incluyendo el nuevo
            partida = await _context.Partidas
                .Include(p => p.Movimientos)
                .Include(p => p.Jugador1)
                .Include(p => p.Jugador2)
                .Include(p => p.TurnoJugador)
                .FirstOrDefaultAsync(p => p.PartidaId == id);

            //Marcar con verde coordenadas ganadoras
            var coordsGanadoras = ObtenerCoordenadasGanadoras(partida!, nuevoMovimiento);
            if (coordsGanadoras.Count >= 4)
            {
                partida!.Estado = "Finalizada";
                partida.Resultado = $"Victoria - {partida.TurnoJugador?.Nombre}";

                // Guardar las coordenadas como strings (ej. "3,2") para que la vista pueda leerlas
                var claves = coordsGanadoras.Select(c => $"{c.fila},{c.col}").ToList();
                TempData["CoordenadasGanadoras"] = JsonSerializer.Serialize(claves);

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Tablero), new { id });
            }

            // 🔴 Verificar empate después de victoria
            if (partida!.Movimientos.Count == 42)
            {
                partida.Estado = "Finalizada";
                partida.Resultado = "Empate";
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Tablero), new { id });
            }

            // Cambiar turno
            partida.TurnoJugadorId = (partida.TurnoJugadorId == partida.Jugador1Id)
                ? partida.Jugador2Id
                : partida.Jugador1Id;

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Tablero), new { id });

        }

        private bool PartidaExists(int id)
        {
            return _context.Partidas.Any(e => e.PartidaId == id);
        }

        private bool VerificarVictoria(Partida partida, Movimiento ultimo)
        {
            int[,] direcciones = new int[,] { { 0, 1 }, { 1, 0 }, { 1, 1 }, { 1, -1 } }; // H, V, D1, D2
            int filas = 6;
            int columnas = 7;

            // Construir tablero temporal
            char[,] tablero = new char[filas, columnas];
            foreach (var mov in partida.Movimientos)
            {
                int col = mov.Columna - 'A';
                tablero[mov.Fila, col] = mov.JugadorId == partida.Jugador1Id ? 'X' : 'O';
            }

            char simbolo = ultimo.JugadorId == partida.Jugador1Id ? 'X' : 'O';
            int filaInicial = ultimo.Fila;
            int colInicial = ultimo.Columna - 'A';

            for (int d = 0; d < 4; d++)
            {
                int dx = direcciones[d, 0];
                int dy = direcciones[d, 1];
                int conteo = 1;

                // Hacia adelante
                int x = filaInicial + dx;
                int y = colInicial + dy;
                while (x >= 0 && x < filas && y >= 0 && y < columnas && tablero[x, y] == simbolo)
                {
                    conteo++;
                    x += dx;
                    y += dy;
                }

                // Hacia atrás
                x = filaInicial - dx;
                y = colInicial - dy;
                while (x >= 0 && x < filas && y >= 0 && y < columnas && tablero[x, y] == simbolo)
                {
                    conteo++;
                    x -= dx;
                    y -= dy;
                }

                if (conteo >= 4)
                    return true;
            }

            return false;
        }

        private bool HayGanador(char[,] tablero, int fila, int columna, char simbolo)
        {
            int[,] direcciones = new int[,]
            {
        { 0, 1 }, // Horizontal
        { 1, 0 }, // Vertical
        { 1, 1 }, // Diagonal descendente
        { 1, -1 } // Diagonal ascendente
            };
            for (int d = 0; d < 4; d++)
            {
                int conteo = 1;

                for (int sentido = -1; sentido <= 1; sentido += 2)
                {
                    int dx = direcciones[d, 0] * sentido;
                    int dy = direcciones[d, 1] * sentido;

                    int x = fila + dx;
                    int y = columna + dy;

                    while (x >= 0 && x < 6 && y >= 0 && y < 7 && tablero[x, y] == simbolo)
                    {
                        conteo++;
                        x += dx;
                        y += dy;
                    }
                }

                if (conteo >= 4)
                    return true;
            }

            return false;
        }
        private List<(int fila, int col)> ObtenerCoordenadasGanadoras(Partida partida, Movimiento ultimo)
        {
            int[,] direcciones = new int[,] { { 0, 1 }, { 1, 0 }, { 1, 1 }, { 1, -1 } };
            int filas = 6;
            int columnas = 7;
            char[,] tablero = new char[filas, columnas];
            foreach (var mov in partida.Movimientos)
            {
                int col = mov.Columna - 'A';
                tablero[mov.Fila, col] = mov.JugadorId == partida.Jugador1Id ? 'X' : 'O';
            }

            char simbolo = ultimo.JugadorId == partida.Jugador1Id ? 'X' : 'O';
            int filaInicial = ultimo.Fila;
            int colInicial = ultimo.Columna - 'A';

            for (int d = 0; d < 4; d++)
            {
                int dx = direcciones[d, 0];
                int dy = direcciones[d, 1];
                List<(int fila, int col)> coords = new() { (filaInicial, colInicial) };

                // Hacia adelante
                int x = filaInicial + dx;
                int y = colInicial + dy;
                while (x >= 0 && x < filas && y >= 0 && y < columnas && tablero[x, y] == simbolo)
                {
                    coords.Add((x, y));
                    x += dx;
                    y += dy;
                }

                // Hacia atrás
                x = filaInicial - dx;
                y = colInicial - dy;
                while (x >= 0 && x < filas && y >= 0 && y < columnas && tablero[x, y] == simbolo)
                {
                    coords.Add((x, y));
                    x -= dx;
                    y -= dy;
                }

                if (coords.Count >= 4)
                    return coords;
            }

            return new List<(int, int)>();
        }
        
        [HttpPost]
        public async Task<IActionResult> Reiniciar(int id)
        {
            // Buscar la partida actual
            var partidaActual = await _context.Partidas
                .Include(p => p.Jugador1)
                .Include(p => p.Jugador2)
                .FirstOrDefaultAsync(p => p.PartidaId == id);

            if (partidaActual == null)
            {
                TempData["Error"] = "No se encontró la partida a reiniciar.";
                return RedirectToAction(nameof(Index));
            }

            // Crear una nueva partida con los mismos jugadores y turno inicial igual que la original
            var nuevaPartida = new Partida
            {
                Jugador1Id = partidaActual.Jugador1Id,
                Jugador2Id = partidaActual.Jugador2Id,
                TurnoJugadorId = partidaActual.TurnoJugadorId,
                Estado = "En Progreso"
            };

            _context.Partidas.Add(nuevaPartida);
            await _context.SaveChangesAsync();

            // Redirigir al tablero de la nueva partida
            return RedirectToAction(nameof(Tablero), new { id = nuevaPartida.PartidaId });
        }
                
            } 
}

