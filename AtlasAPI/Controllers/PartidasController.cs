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
                .Take(10) // Mostrar las últimas 10 partidas
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
            
            // --- VALIDACIÓN ADICIONAL PARA DETECTAR DATOS MALFORMADOS ---
            // Verificamos si los valores recibidos parecen ser rutas de archivo en lugar de datos válidos.
            if (IsPotentiallyFilePath(partida.UsuarioId.ToString()))
            {
                return BadRequest("Invalid data received for UsuarioId. It appears to be a file path.");
            }
            if (IsPotentiallyFilePath(partida.Aciertos.ToString()))
            {
                return BadRequest("Invalid data received for Aciertos. It appears to be a file path.");
            }
            if (IsPotentiallyFilePath(partida.TotalPreguntas.ToString()))
            {
                return BadRequest("Invalid data received for TotalPreguntas. It appears to be a file path.");
            }
            if (IsPotentiallyFilePath(partida.Siglo.ToString()))
            {
                return BadRequest("Invalid data received for Siglo. It appears to be a file path.");
            }
            if (IsPotentiallyFilePath(partida.Fecha.ToString()))
            {
                return BadRequest("Invalid data received for Fecha. It appears to be a file path.");
            }
            // --- FIN DE VALIDACIÓN ---

            // Aseguramos que la fecha se establezca en el servidor para evitar problemas de formato o zona horaria desde el cliente.
            partida.Fecha = DateTime.UtcNow;
            Console.WriteLine($"[DEBUG]   Fecha (final antes de Add): {partida.Fecha}");
            
            _context.Partidas.Add(partida);
            await _context.SaveChangesAsync();

            return Ok(partida);
        }

        // Método auxiliar para verificar si una cadena parece una ruta de archivo.
        private bool IsPotentiallyFilePath(string value)
        {
            if (string.IsNullOrEmpty(value)) return false;

            if (value.Contains('\\') || value.Contains('/') || value.Contains(':') || value.Contains(".."))
            {
                if (value.StartsWith("@.") || value.StartsWith("@ ")) return true;
                if (value.Length >= 3 && char.IsLetter(value[0]) && value[1] == ':' && value[2] == '\\') return true;
                if ((value.Contains('\\') || value.Contains('/')) && !value.StartsWith("-")) return true;
                if (value.Contains("..")) return true;
            }
            return false;
        }
    }
}