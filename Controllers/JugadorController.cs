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
    public class JugadorController : Controller
    {
        private readonly ApplicationDbContext _context;

        public JugadorController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Jugador
        public async Task<IActionResult> Index()
        {
            return View(await _context.Jugadores.ToListAsync());
        }

        // GET: Jugador/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var jugador = await _context.Jugadores
                .FirstOrDefaultAsync(m => m.JugadorId == id);
            if (jugador == null)
            {
                return NotFound();
            }

            return View(jugador);
        }

        // GET: Jugador/Create
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Nombre")] Jugador jugador)
        {
        if (ModelState.IsValid)
        {
        // Asignar valores por defecto
        jugador.Marcador = 0;
        jugador.Ganadas = 0;
        jugador.Perdidas = 0;
        jugador.Empatadas = 0;
            _context.Add(jugador);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(jugador);
        }

        // GET: Jugador/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var jugador = await _context.Jugadores.FindAsync(id);
            if (jugador == null)
            {
                return NotFound();
            }
            return View(jugador);
        }

        // POST: Jugador/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("JugadorId,Nombre,Marcador,Ganadas,Perdidas,Empatadas")] Jugador jugador)
        {
            if (id != jugador.JugadorId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(jugador);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!JugadorExists(jugador.JugadorId))
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
            return View(jugador);
        }

        // GET: Jugador/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var jugador = await _context.Jugadores
                .FirstOrDefaultAsync(m => m.JugadorId == id);
            if (jugador == null)
            {
                return NotFound();
            }

            return View(jugador);
        }

        // POST: Jugador/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var jugador = await _context.Jugadores.FindAsync(id);
            if (jugador != null)
            {
                _context.Jugadores.Remove(jugador);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Jugador/Ranking
        public async Task<IActionResult> Ranking()
        {
            var jugadores = await _context.Jugadores
            .Include(j => j.PartidasComoJugador1)
            .Include(j => j.PartidasComoJugador2)
            .ToListAsync();
            foreach (var jugador in jugadores)
            {
                int ganadas = 0, perdidas = 0, empatadas = 0;

                var todas = jugador.PartidasComoJugador1.Concat(jugador.PartidasComoJugador2).Where(p => p.Estado == "Finalizada");

                foreach (var partida in todas)
                {
                    if (partida.Resultado?.StartsWith("Victoria") == true)
                    {
                        if (partida.Resultado.Contains(jugador.Nombre))
                            ganadas++;
                        else
                            perdidas++;
                    }
                    else if (partida.Resultado == "Empate")
                    {
                        empatadas++;
                    }
                }

                jugador.Ganadas = ganadas;
                jugador.Perdidas = perdidas;
                jugador.Empatadas = empatadas;
                jugador.Marcador = ganadas - perdidas;

                _context.Update(jugador); // Guardamos cambios
            }

            await _context.SaveChangesAsync();

            var ranking = jugadores.OrderByDescending(j => j.Marcador).ThenBy(j => j.Nombre).ToList();
            return View(ranking);
        }

        private bool JugadorExists(int id)
        {
            return _context.Jugadores.Any(e => e.JugadorId == id);
        }
    }
}
