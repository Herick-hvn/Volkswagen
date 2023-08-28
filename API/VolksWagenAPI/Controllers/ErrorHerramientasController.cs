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
    public class ErrorHerramientasController : ControllerBase
    {
        private readonly VolksWagenContext _context;

        public ErrorHerramientasController(VolksWagenContext context)
        {
            _context = context;
        }

        // GET: api/ErrorHerramientas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ErrorHerramientum>>> GetErrorHerramienta()
        {
          if (_context.ErrorHerramienta == null)
          {
              return NotFound();
          }
            return await _context.ErrorHerramienta.ToListAsync();
        }

        // GET: api/ErrorHerramientas/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ErrorHerramientum>> GetErrorHerramientum(int id)
        {
          if (_context.ErrorHerramienta == null)
          {
              return NotFound();
          }
            var errorHerramientum = await _context.ErrorHerramienta.FindAsync(id);

            if (errorHerramientum == null)
            {
                return NotFound();
            }

            return errorHerramientum;
        }

        // PUT: api/ErrorHerramientas/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutErrorHerramientum(int id, ErrorHerramientum errorHerramientum)
        {
            if (id != errorHerramientum.Id)
            {
                return BadRequest();
            }

            _context.Entry(errorHerramientum).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ErrorHerramientumExists(id))
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

        // POST: api/ErrorHerramientas
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ErrorHerramientum>> PostErrorHerramientum(ErrorHerramientum errorHerramientum)
        {
          if (_context.ErrorHerramienta == null)
          {
              return Problem("Entity set 'VolksWagenContext.ErrorHerramienta'  is null.");
          }
            _context.ErrorHerramienta.Add(errorHerramientum);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetErrorHerramientum", new { id = errorHerramientum.Id }, errorHerramientum);
        }

        // DELETE: api/ErrorHerramientas/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteErrorHerramientum(int id)
        {
            if (_context.ErrorHerramienta == null)
            {
                return NotFound();
            }

            var errorHerramientum = await _context.ErrorHerramienta.FindAsync(id);
            if (errorHerramientum == null)
            {
                return NotFound();
            }

            // Cambiar el estado a "0" en lugar de eliminar
            errorHerramientum.Estado = "0";
            _context.Entry(errorHerramientum).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // ... (otros métodos del controlador)

        // POST: api/ErrorHerramientas/SubirNivelErrorHerramienta/{id}
        [HttpPost("SubirNivelErrorHerramienta/{id}")]
        public async Task<IActionResult> SubirNivelErrorHerramienta(int id)
        {
            if (_context.ErrorHerramienta == null)
            {
                return NotFound();
            }

            var errorHerramientum = await _context.ErrorHerramienta.FindAsync(id);
            if (errorHerramientum == null)
            {
                return NotFound();
            }

            // Incrementar el nivel del error de la herramienta
            errorHerramientum.Nivel += 1;
            _context.Entry(errorHerramientum).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }


        // GET: api/ErrorHerramientas/ByUserRole/{userId}
        [HttpGet("ByUserRole/{userId}")]
        public async Task<ActionResult<IEnumerable<ErrorHerramientum>>> GetErrorHerramientasByUserRole(int userId)
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

            var errorHerramientas = await _context.ErrorHerramienta
                .Where(e => e.Nivel == nivelUsuario.Value)
                .ToListAsync();

            return errorHerramientas;
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


        private bool ErrorHerramientumExists(int id)
        {
            return (_context.ErrorHerramienta?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
