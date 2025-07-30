using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Connect4.Web.Models;

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
            ViewData["TurnoJugadorId"] = jugadores;
            return View();
        }

        // POST: Partida/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Jugador1Id,Jugador2Id,TurnoJugadorId")] Partida partida)
        {
            // Validación: los jugadores no pueden ser el mismo
            if (partida.Jugador1Id == partida.Jugador2Id)
            {
                ModelState.AddModelError(string.Empty, "Los jugadores deben ser distintos.");
            }

            if (ModelState.IsValid)
            {
                // Asignación automática de campos
                partida.FechaHora = DateTime.Now;
                partida.Estado = "EnCurso";
                partida.Resultado = "EnCurso";

                _context.Add(partida);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Recargar los dropdowns si hay error
            var jugadores = new SelectList(_context.Jugadores, "JugadorId", "Nombre");
            ViewData["Jugador1Id"] = jugadores;
            ViewData["Jugador2Id"] = jugadores;
            ViewData["TurnoJugadorId"] = jugadores;

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
            // Obtener partida con movimientos y jugadores relacionados
            var partida = await _context.Partidas
                .Include(p => p.Movimientos)
                .Include(p => p.Jugador1)
                .Include(p => p.Jugador2)
                .Include(p => p.TurnoJugador)
                .FirstOrDefaultAsync(p => p.PartidaId == id);

            if (partida == null)
                return NotFound();

            // Crear matriz 6 filas x 7 columnas
            char[,] tablero = new char[6, 7];
            for (int fila = 0; fila < 6; fila++)
            {
                for (int col = 0; col < 7; col++)
                {
                    tablero[fila, col] = ' ';
                }
            }

            // Llenar la matriz con los movimientos
            foreach (var mov in partida.Movimientos)
            {
                int columnaIndice = mov.Columna - 'A'; // Convierte 'A'-'G' a 0-6
                tablero[mov.Fila, columnaIndice] = mov.JugadorId == partida.Jugador1Id ? 'X' : 'O';
            }

            // Pasar tablero y partida a la vista
            ViewBag.Tablero = tablero;
            ViewBag.Partida = partida;

            return View();
        }








        // POST: Partida/Jugar/5
        [HttpPost]
        public async Task<IActionResult> Jugar(int id, char columna)
        {
            // Buscar partida con movimientos
            var partida = await _context.Partidas
                .Include(p => p.Movimientos)
                .FirstOrDefaultAsync(p => p.PartidaId == id);

            if (partida == null || partida.Estado != "En progreso")
                return NotFound();

            // Validar que la columna sea válida (A-G)
            if (columna < 'A' || columna > 'G')
            {
                TempData["Error"] = "Columna inválida.";
                return RedirectToAction(nameof(Tablero), new { id });
            }

            // Obtener movimientos en esa columna
            var movimientosColumna = partida.Movimientos
                .Where(m => m.Columna == columna)
                .OrderBy(m => m.Fila)
                .ToList();

            if (movimientosColumna.Count >= 6)
            {
                TempData["Error"] = "La columna está llena.";
                return RedirectToAction(nameof(Tablero), new { id });
            }

            // La fila para la nueva ficha es la mínima fila libre (fila 0 es la más baja)
            int fila = movimientosColumna.Count; 

            // Obtener el número de turno (OrdenTurno)
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

            // Cambiar turno al otro jugador
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
        
        


    }
}

