using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AtlasAPI.Context;
using AtlasAPI.Models;

namespace AtlasAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LogrosController : ControllerBase
    {
        private readonly ContextoAtlas _context;

        public LogrosController(ContextoAtlas context)
        {
            _context = context;
        }

        // GET: api/Logros
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Logro>>> GetLogros()
        {
            return await _context.Logros.ToListAsync();
        }

        // GET: api/Logros/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Logro>> GetLogro(int id)
        {
            var logro = await _context.Logros.FindAsync(id);

            if (logro == null)
            {
                return NotFound();
            }

            return logro;
        }

        // PUT: api/Logros/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutLogro(int id, Logro logro)
        {
            if (id != logro.Id)
            {
                return BadRequest();
            }

            _context.Entry(logro).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LogroExists(id))
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

        // POST: api/Logros
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Logro>> PostLogro(Logro logro)
        {
            _context.Logros.Add(logro);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetLogro", new { id = logro.Id }, logro);
        }

        // DELETE: api/Logros/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLogro(int id)
        {
            var logro = await _context.Logros.FindAsync(id);
            if (logro == null)
            {
                return NotFound();
            }

            _context.Logros.Remove(logro);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET: api/Logros/usuario/5
        [HttpGet("usuario/{id}")]
        public async Task<ActionResult<IEnumerable<object>>> GetLogrosByUsuario(int id)
        {
            var todosLosLogros = await _context.Logros.ToListAsync();
            var logrosDelUsuario = await _context.UsuariosLogros
                .Where(ul => ul.UsuarioId == id)
                .Select(ul => ul.LogroId)
                .ToListAsync();

            var resultado = todosLosLogros.Select(l => new 
            {
                Id = l.Id,
                Titulo = l.Titulo,
                Descripcion = l.Descripcion,
                Icono = l.Icono,
                Desbloqueado = logrosDelUsuario.Contains(l.Id)
            });

            return Ok(resultado);
        }

        private bool LogroExists(int id)
        {
            return _context.Logros.Any(e => e.Id == id);
        }
    }
}
