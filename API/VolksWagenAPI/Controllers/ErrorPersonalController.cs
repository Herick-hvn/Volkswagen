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
    public class ErrorPersonalController : ControllerBase
    {
        private readonly VolksWagenContext _context;

        public ErrorPersonalController(VolksWagenContext context)
        {
            _context = context;
        }

        // GET: api/ErrorPersonal
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ErrorPersonal>>> GetErrorPersonals()
        {
          if (_context.ErrorPersonals == null)
          {
              return NotFound();
          }
            return await _context.ErrorPersonals.ToListAsync();
        }

        // GET: api/ErrorPersonal/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ErrorPersonal>> GetErrorPersonal(int id)
        {
          if (_context.ErrorPersonals == null)
          {
              return NotFound();
          }
            var errorPersonal = await _context.ErrorPersonals.FindAsync(id);

            if (errorPersonal == null)
            {
                return NotFound();
            }

            return errorPersonal;
        }

        // PUT: api/ErrorPersonal/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutErrorPersonal(int id, ErrorPersonal errorPersonal)
        {
            if (id != errorPersonal.Id)
            {
                return BadRequest();
            }

            _context.Entry(errorPersonal).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ErrorPersonalExists(id))
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

        // POST: api/ErrorPersonal
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ErrorPersonal>> PostErrorPersonal(ErrorPersonal errorPersonal)
        {
          if (_context.ErrorPersonals == null)
          {
              return Problem("Entity set 'VolksWagenContext.ErrorPersonals'  is null.");
          }
            _context.ErrorPersonals.Add(errorPersonal);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetErrorPersonal", new { id = errorPersonal.Id }, errorPersonal);
        }

        // DELETE: api/ErrorPersonal/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteErrorPersonal(int id)
        {
            if (_context.ErrorPersonals == null)
            {
                return NotFound();
            }

            var errorPersonal = await _context.ErrorPersonals.FindAsync(id);
            if (errorPersonal == null)
            {
                return NotFound();
            }

            // Cambiar el estado a "0" en lugar de eliminar
            errorPersonal.Estado = "0";
            _context.Entry(errorPersonal).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // ... (otros métodos del controlador)

        // POST: api/ErrorPersonal/SubirNivelErrorPersonal/{id}
        [HttpPost("SubirNivelErrorPersonal/{id}")]
        public async Task<IActionResult> SubirNivelErrorPersonal(int id)
        {
            if (_context.ErrorPersonals == null)
            {
                return NotFound();
            }

            var errorPersonal = await _context.ErrorPersonals.FindAsync(id);
            if (errorPersonal == null)
            {
                return NotFound();
            }

            // Incrementar el nivel del error personal
            errorPersonal.Nivel += 1;
            _context.Entry(errorPersonal).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }



        // GET: api/ErrorPersonal/ByUserRole/{userId}
        [HttpGet("ByUserRole/{userId}")]
        public async Task<ActionResult<IEnumerable<ErrorPersonal>>> GetErrorPersonalsByUserRole(int userId)
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

            var erroresPersonal = await _context.ErrorPersonals
                .Where(e => e.Nivel == nivelUsuario.Value)
                .ToListAsync();

            return erroresPersonal;
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
        private bool ErrorPersonalExists(int id)
        {
            return (_context.ErrorPersonals?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
