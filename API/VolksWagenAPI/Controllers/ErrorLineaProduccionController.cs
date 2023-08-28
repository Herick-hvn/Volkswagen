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
    public class ErrorLineaProduccionController : ControllerBase
    {
        private readonly VolksWagenContext _context;

        public ErrorLineaProduccionController(VolksWagenContext context)
        {
            _context = context;
        }

        // GET: api/ErrorLineaProduccion
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ErrorLineaProduccion>>> GetErrorLineaProduccions()
        {
          if (_context.ErrorLineaProduccions == null)
          {
              return NotFound();
          }
            return await _context.ErrorLineaProduccions.ToListAsync();
        }

        // GET: api/ErrorLineaProduccion/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ErrorLineaProduccion>> GetErrorLineaProduccion(int id)
        {
          if (_context.ErrorLineaProduccions == null)
          {
              return NotFound();
          }
            var errorLineaProduccion = await _context.ErrorLineaProduccions.FindAsync(id);

            if (errorLineaProduccion == null)
            {
                return NotFound();
            }

            return errorLineaProduccion;
        }

        // PUT: api/ErrorLineaProduccion/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutErrorLineaProduccion(int id, ErrorLineaProduccion errorLineaProduccion)
        {
            if (id != errorLineaProduccion.Id)
            {
                return BadRequest();
            }

            _context.Entry(errorLineaProduccion).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ErrorLineaProduccionExists(id))
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

        // POST: api/ErrorLineaProduccion
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ErrorLineaProduccion>> PostErrorLineaProduccion(ErrorLineaProduccion errorLineaProduccion)
        {
          if (_context.ErrorLineaProduccions == null)
          {
              return Problem("Entity set 'VolksWagenContext.ErrorLineaProduccions'  is null.");
          }
            _context.ErrorLineaProduccions.Add(errorLineaProduccion);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetErrorLineaProduccion", new { id = errorLineaProduccion.Id }, errorLineaProduccion);
        }

        // DELETE: api/ErrorLineaProduccion/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteErrorLineaProduccion(int id)
        {
            if (_context.ErrorLineaProduccions == null)
            {
                return NotFound();
            }

            var errorLineaProduccion = await _context.ErrorLineaProduccions.FindAsync(id);
            if (errorLineaProduccion == null)
            {
                return NotFound();
            }

            // Cambiar el estado a "0" en lugar de eliminar
            errorLineaProduccion.Estado = "0";
            _context.Entry(errorLineaProduccion).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // ... (otros métodos del controlador)

        // POST: api/ErrorLineaProduccion/SubirNivelErrorLineaProduccion/{id}
        [HttpPost("SubirNivelErrorLineaProduccion/{id}")]
        public async Task<IActionResult> SubirNivelErrorLineaProduccion(int id)
        {
            if (_context.ErrorLineaProduccions == null)
            {
                return NotFound();
            }

            var errorLineaProduccion = await _context.ErrorLineaProduccions.FindAsync(id);
            if (errorLineaProduccion == null)
            {
                return NotFound();
            }

            // Incrementar el nivel del error de la línea de producción
            errorLineaProduccion.Nivel += 1;
            _context.Entry(errorLineaProduccion).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }


        // GET: api/ErrorLineaProduccion/ByUserRole/{userId}
        [HttpGet("ByUserRole/{userId}")]
        public async Task<ActionResult<IEnumerable<ErrorLineaProduccion>>> GetErrorLineaProduccionsByUserRole(int userId)
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

            var erroresLineaProduccion = await _context.ErrorLineaProduccions
                .Where(e => e.Nivel == nivelUsuario.Value)
                .ToListAsync();

            return erroresLineaProduccion;
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


        private bool ErrorLineaProduccionExists(int id)
        {
            return (_context.ErrorLineaProduccions?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
