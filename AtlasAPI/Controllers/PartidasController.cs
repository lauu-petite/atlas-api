using AtlasAPI.Context;
using AtlasAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AtlasAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PartidasController : ControllerBase
    {
        private readonly ContextoAtlas _context;

        public PartidasController(ContextoAtlas context)
        {
            _context = context;
        }

        // 1. OBTENER HISTORIAL DE UN USUARIO
        [HttpGet("usuario/{usuarioId}")]
        public async Task<ActionResult<IEnumerable<Partida>>> GetHistorial(int usuarioId)
        {
            return await _context.Partidas
                .Where(p => p.UsuarioId == usuarioId)
                .OrderByDescending(p => p.Fecha)
                .Take(10) // Mostrar las últimas 10 partidas
                .ToListAsync();
        }

        // 2. GUARDAR RESULTADO DE UNA PARTIDA
        [HttpPost]
        public async Task<ActionResult<Partida>> PostPartida(Partida partida)
        {
            partida.Fecha = DateTime.Now; // Asegurar que la fecha es la actual
            _context.Partidas.Add(partida);
            await _context.SaveChangesAsync();

            return Ok(partida);
        }
    }
}