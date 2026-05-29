using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AtlasAPI.Context;
using AtlasAPI.Models;
using AtlasAPI.Services;

namespace AtlasAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PreguntasController : ControllerBase
    {
        private readonly ContextoAtlas _context;
        private readonly HistoriaService _aiService;

        public PreguntasController(ContextoAtlas context, HistoriaService aiService)
        {
            _context = context;
            _aiService = aiService;
        }

        // 1. Obtener una lista de preguntas aleatorias (con filtros)
        [HttpGet("randomList")]
        public async Task<ActionResult<IEnumerable<PreguntaQuiz>>> GetRandomList(int? siglo, string? categoria)
        {
            IQueryable<PreguntaQuiz> query = _context.Preguntas;

            if (siglo.HasValue)
                query = query.Where(p => p.Siglo == siglo.Value);

            if (!string.IsNullOrEmpty(categoria))
                query = query.Where(p => p.Categoria == categoria);
            
            var listaCompleta = await query.ToListAsync();
            
            // Si no hay suficientes, intentar generar con IA
            if (listaCompleta.Count < 6 && siglo.HasValue && !string.IsNullOrEmpty(categoria))
            {
                // Generar varias veces hasta tener 6 (o todas las posibles)
                for (int i = 0; i < 6; i++) {
                    var jsonPregunta = await _aiService.GenerarPreguntaAleatoria(siglo.Value, categoria);
                    // (Lógica de limpieza omitida por brevedad en este ejemplo)
                    try {
                        var nueva = System.Text.Json.JsonSerializer.Deserialize<PreguntaQuiz>(jsonPregunta, new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                        if (nueva != null) {
                            _context.Preguntas.Add(nueva);
                            listaCompleta.Add(nueva);
                        }
                    } catch {}
                }
                await _context.SaveChangesAsync();
            }

            // Mezclar y tomar 6
            var random = new Random();
            var seleccionadas = listaCompleta.OrderBy(x => random.Next()).Take(6).ToList();

            return Ok(seleccionadas);
        }

        // 2. IA -> Generador de Descripciones Fascinantes
        [HttpGet("ia/descripcion/{monumento}")]
        public async Task<IActionResult> GetDescripcionIA(string monumento)
        {
            var descripcion = await _aiService.ObtenerExplicacionIA(0, monumento);
            return Ok(new { monumento, descripcion });
        }
    }
}