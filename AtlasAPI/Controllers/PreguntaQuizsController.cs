using AtlasAPI.Context;
using AtlasAPI.Models;
using AtlasAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AtlasAPI.Controllers
{
    //CONTROLADOR 1: Preguntas que ya están en la bbdd
    [Route("api/[controller]")]
    [ApiController]
    public class PreguntaQuizsController : ControllerBase
    {
        private readonly ContextoAtlas _context;

        public PreguntaQuizsController(ContextoAtlas context)
        {
            _context = context;
        }

        // GET: api/PreguntaQuizs
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PreguntaQuiz>>> GetPreguntas()
        {
            return await _context.Preguntas.ToListAsync();
        }

        // GET: api/PreguntaQuizs/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PreguntaQuiz>> GetPreguntaQuiz(int id)
        {
            var preguntaQuiz = await _context.Preguntas.FindAsync(id);

            if (preguntaQuiz == null)
            {
                return NotFound();
            }

            return preguntaQuiz;
        }

        // PUT: api/PreguntaQuizs/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPreguntaQuiz(int id, PreguntaQuiz preguntaQuiz)
        {
            if (id != preguntaQuiz.Id)
            {
                return BadRequest();
            }

            _context.Entry(preguntaQuiz).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PreguntaQuizExists(id))
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

        // POST: api/PreguntaQuizs
        [HttpPost]
        public async Task<ActionResult<PreguntaQuiz>> PostPreguntaQuiz(PreguntaQuiz preguntaQuiz)
        {
            _context.Preguntas.Add(preguntaQuiz);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPreguntaQuiz", new { id = preguntaQuiz.Id }, preguntaQuiz);
        }

        // DELETE: api/PreguntaQuizs/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePreguntaQuiz(int id)
        {
            var preguntaQuiz = await _context.Preguntas.FindAsync(id);
            if (preguntaQuiz == null)
            {
                return NotFound();
            }

            _context.Preguntas.Remove(preguntaQuiz);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PreguntaQuizExists(int id)
        {
            return _context.Preguntas.Any(e => e.Id == id);
        }
    }

    // CONTROLADOR 2: Uso IA
    [Route("api/[controller]")]
    [ApiController]
    public class QuizController : ControllerBase
    {
        private readonly HistoriaService _aiService;
        private readonly ContextoAtlas _context;

        public QuizController(HistoriaService aiService, ContextoAtlas context)
        {
            _aiService = aiService;
            _context = context;
        }

        [HttpGet("generar/{usuarioId}/{tema}")]
        public async Task<IActionResult> GetPregunta(int usuarioId, string tema)
        {
            var usuario = await _context.Usuarios.FindAsync(usuarioId);
            if (usuario == null) return NotFound("Usuario no encontrado en la base de datos");

            // Nivel del usuario se toma como Siglo aproximado para la IA si no se especifica
            var pregunta = await _aiService.GenerarPreguntaAleatoria(usuario.Puntos / 100, tema); 
            return Ok(pregunta);
        }
    }
}