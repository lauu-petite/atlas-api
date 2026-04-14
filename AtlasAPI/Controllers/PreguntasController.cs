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

        // 1. Obtener una pregunta aleatoria (con filtros y Quiz Infinito)
        [HttpGet("random")]
        public async Task<ActionResult<PreguntaQuiz>> GetRandom(int? siglo, string? categoria)
        {
            IQueryable<PreguntaQuiz> query = _context.Preguntas;

            if (siglo.HasValue)
                query = query.Where(p => p.Siglo == siglo.Value);
            
            if (!string.IsNullOrEmpty(categoria))
                query = query.Where(p => p.Categoria == categoria);

            var listaPreguntas = await query.ToListAsync();
            
            if (listaPreguntas.Count == 0) 
            {
                // MODO QUIZ INFINITO: Generar con IA si no hay en DB
                if (siglo.HasValue && !string.IsNullOrEmpty(categoria))
                {
                    var jsonPregunta = await _aiService.GenerarPreguntaAleatoria(siglo.Value, categoria);
                    
                    // Limpieza de Markdown si Gemini lo incluye
                    if (jsonPregunta.StartsWith("```json"))
                        jsonPregunta = jsonPregunta.Replace("```json", "").Replace("```", "").Trim();
                    else if (jsonPregunta.StartsWith("```"))
                        jsonPregunta = jsonPregunta.Replace("```", "").Trim();

                    try 
                    {
                        var nuevaPregunta = System.Text.Json.JsonSerializer.Deserialize<PreguntaQuiz>(jsonPregunta, new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                        if (nuevaPregunta != null)
                        {
                            _context.Preguntas.Add(nuevaPregunta);
                            await _context.SaveChangesAsync();
                            return Ok(nuevaPregunta);
                        }
                    }
                    catch { /* Fallback si falla el JSON */ }
                }
                
                return NotFound("No se encontraron preguntas y no se pudo generar una nueva.");
            }

            var random = new Random();
            var pregunta = listaPreguntas[random.Next(0, listaPreguntas.Count)];

            return Ok(pregunta);
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