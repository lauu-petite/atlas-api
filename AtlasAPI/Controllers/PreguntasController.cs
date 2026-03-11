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

        // 1. Obtener una pregunta aleatoria
        [HttpGet("random")]
        public async Task<ActionResult<PreguntaQuiz>> GetRandom()
        {
            var total = await _context.Preguntas.CountAsync();
            if (total == 0) return NotFound("No hay preguntas en la base de datos.");

            var random = new Random();
            var skip = random.Next(0, total);

            var pregunta = await _context.Preguntas.Skip(skip).FirstOrDefaultAsync();
            
            if (pregunta != null && string.IsNullOrEmpty(pregunta.Explicacion))
            {
                // Si no tiene explicación, la pedimos a Gemini y la guardamos
                var prompt = $"Como un historiador experto de Atlas, proporciona una explicación breve (máximo 2 líneas) y fascinante sobre el tema '{pregunta.Tema}' " +
                             $"relacionado con esta pregunta: '{pregunta.Enunciado}'. El tono debe ser educativo y sorprendente.";
                
                pregunta.Explicacion = await _aiService.GenerarTexto(prompt);
                await _context.SaveChangesAsync();
            }

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