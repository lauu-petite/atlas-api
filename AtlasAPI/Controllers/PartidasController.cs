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
            var historial = await _context.Partidas
                .Where(p => p.UsuarioId == usuarioId)
                .OrderByDescending(p => p.Fecha)
                .ToListAsync();

            // Log para depuración detallado al obtener historial
            Console.WriteLine($"[DEBUG] Obteniendo historial para UsuarioId: {usuarioId}. Se encontraron {historial.Count} partidas.");
            foreach (var p in historial)
            {
                Console.WriteLine($"[DEBUG] Partida ID: {p.Id}, Aciertos: {p.Aciertos}, TotalPreguntas: {p.TotalPreguntas}, Siglo: {p.Siglo}, Fecha: {p.Fecha}");
            }

            return Ok(historial);
        }

        // 2. GUARDAR RESULTADO DE UNA PARTIDA
        [HttpPost]
        public async Task<ActionResult<Partida>> PostPartida(Partida partida)
        {
            // Log para depuración detallado al guardar
            Console.WriteLine($"[DEBUG] Intentando guardar Partida:");
            Console.WriteLine($"[DEBUG]   UsuarioId: {partida.UsuarioId}");
            Console.WriteLine($"[DEBUG]   Aciertos: {partida.Aciertos}");
            Console.WriteLine($"[DEBUG]   TotalPreguntas: {partida.TotalPreguntas}");
            Console.WriteLine($"[DEBUG]   Siglo: {partida.Siglo}");
            Console.WriteLine($"[DEBUG]   Fecha (antes de asignar): {partida.Fecha}");
            
            // --- VALIDACIÓN DE USUARIO VÁLIDO ---
            if (partida.UsuarioId <= 0)
            {
                Console.WriteLine($"[ERROR] Intento de guardar partida con UsuarioId inválido: {partida.UsuarioId}");
                return BadRequest("El UsuarioId debe ser un valor positivo válido.");
            }

            // Aseguramos que la fecha se establezca en el servidor para evitar problemas de formato o zona horaria desde el cliente.
            partida.Fecha = DateTime.UtcNow;
            Console.WriteLine($"[DEBUG]   Fecha (final antes de Add): {partida.Fecha}");
            
            _context.Partidas.Add(partida);
            await _context.SaveChangesAsync();

            return Ok(partida);
        }
    }
}