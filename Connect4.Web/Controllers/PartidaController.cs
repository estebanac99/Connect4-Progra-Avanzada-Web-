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
        public async Task<IActionResult> Create([Bind("PartidaId,FechaHora,Jugador1Id,Jugador2Id,Estado,TurnoJugadorId,Resultado")] Partida partida)
        {
            if (ModelState.IsValid)
            {
                _context.Add(partida);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Jugador1Id"] = new SelectList(_context.Jugadores, "JugadorId", "JugadorId", partida.Jugador1Id);
            ViewData["Jugador2Id"] = new SelectList(_context.Jugadores, "JugadorId", "JugadorId", partida.Jugador2Id);
            ViewData["TurnoJugadorId"] = new SelectList(_context.Jugadores, "JugadorId", "JugadorId", partida.TurnoJugadorId);
            return View(partida);
        }

        // GET: Partida/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var partida = await _context.Partidas.FindAsync(id);
            if (partida == null)
            {
                return NotFound();
            }
            ViewData["Jugador1Id"] = new SelectList(_context.Jugadores, "JugadorId", "JugadorId", partida.Jugador1Id);
            ViewData["Jugador2Id"] = new SelectList(_context.Jugadores, "JugadorId", "JugadorId", partida.Jugador2Id);
            ViewData["TurnoJugadorId"] = new SelectList(_context.Jugadores, "JugadorId", "JugadorId", partida.TurnoJugadorId);
            return View(partida);
        }

        // POST: Partida/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PartidaId,FechaHora,Jugador1Id,Jugador2Id,Estado,TurnoJugadorId,Resultado")] Partida partida)
        {
            if (id != partida.PartidaId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(partida);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PartidaExists(partida.PartidaId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["Jugador1Id"] = new SelectList(_context.Jugadores, "JugadorId", "JugadorId", partida.Jugador1Id);
            ViewData["Jugador2Id"] = new SelectList(_context.Jugadores, "JugadorId", "JugadorId", partida.Jugador2Id);
            ViewData["TurnoJugadorId"] = new SelectList(_context.Jugadores, "JugadorId", "JugadorId", partida.TurnoJugadorId);
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

        private bool PartidaExists(int id)
        {
            return _context.Partidas.Any(e => e.PartidaId == id);
        }
    }
}
