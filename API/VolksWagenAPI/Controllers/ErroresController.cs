using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VolkswagenAPI.Models;

namespace VolkswagenAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ErroresController : ControllerBase
    {
        private readonly VolksWagenContext _context;

        public ErroresController(VolksWagenContext context)
        {
            _context = context;
        }

        // GET: api/Errores
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Errore>>> GetErrores()
        {
          if (_context.Errores == null)
          {
              return NotFound();
          }
            return await _context.Errores.ToListAsync();
        }

        // GET: api/Errores/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Errore>> GetErrore(int id)
        {
          if (_context.Errores == null)
          {
              return NotFound();
          }
            var errore = await _context.Errores.FindAsync(id);

            if (errore == null)
            {
                return NotFound();
            }

            return errore;
        }

        // PUT: api/Errores/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutErrore(int id, Errore errore)
        {
            if (id != errore.Id)
            {
                return BadRequest();
            }

            _context.Entry(errore).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ErroreExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Errores
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Errore>> PostErrore(Errore errore)
        {
          if (_context.Errores == null)
          {
              return Problem("Entity set 'VolksWagenContext.Errores'  is null.");
          }
            _context.Errores.Add(errore);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetErrore", new { id = errore.Id }, errore);
        }

        // DELETE: api/Errores/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteErrore(int id)
        {
            if (_context.Errores == null)
            {
                return NotFound();
            }

            var errore = await _context.Errores.FindAsync(id);
            if (errore == null)
            {
                return NotFound();
            }

            // Cambiar el estado a 0 en lugar de eliminar
            errore.Estado = "0";
            _context.Entry(errore).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }


        // POST: api/Errores/SubirNivelError/{id}
        [HttpPost("SubirNivelError/{id}")]
        public async Task<IActionResult> SubirNivelError(int id)
        {
            if (_context.Errores == null)
            {
                return NotFound();
            }

            var errore = await _context.Errores.FindAsync(id);
            if (errore == null)
            {
                return NotFound();
            }

            // Incrementar el nivel del error
            errore.Nivel += 1;
            _context.Entry(errore).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }




        // GET: api/Errores/ByUserRole/{userId}
        [HttpGet("ByUserRole/{userId}")]
        public async Task<ActionResult<IEnumerable<Errore>>> GetErroresByUserRole(int userId)
        {
            var usuario = await _context.Usuarios.FindAsync(userId);
            if (usuario == null)
            {
                return NotFound("Usuario no encontrado");
            }

            var nivelUsuario = GetNivelFromRol(usuario.Rol);
            if (nivelUsuario == null)
            {
                return BadRequest("Rol del usuario no válido");
            }

            var errores = await _context.Errores
                .Where(e => e.Nivel == nivelUsuario.Value)
                .ToListAsync();

            return errores;
        }

        // Método auxiliar para obtener el nivel según el rol
        private int? GetNivelFromRol(string? rol)
        {
            switch (rol)
            {
                case "colaborador":
                    return 1;
                case "supervisor":
                    return 2;
                case "directivo":
                    return 3;
                default:
                    return null; // Si el rol no coincide, nivel nulo
            }
        }




        private bool ErroreExists(int id)
        {
            return (_context.Errores?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
