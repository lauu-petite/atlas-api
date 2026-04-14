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
    [Route("api/admin/eventos")]
    [ApiController]
    public class EventosController : ControllerBase
    {
        private readonly ContextoAtlas _context;
        private readonly HistoriaService _historiaService;
        public EventosController(ContextoAtlas context, HistoriaService historiaService)
        {
            _context = context;
            _historiaService = historiaService;
        }

        // GET: api/Eventos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Evento>>> GetEventos()
        {
            return await _context.Eventos.ToListAsync();
        }

        // GET: api/Eventos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Evento>> GetEvento(int id)
        {
            var evento = await _context.Eventos.FindAsync(id);

            if (evento == null)
            {
                return NotFound();
            }

            return evento;
        }

        // PUT: api/Eventos/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEvento(int id, Evento evento)
        {
            if (id != evento.Id)
            {
                return BadRequest();
            }

            _context.Entry(evento).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EventoExists(id))
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

        // POST: api/Eventos
        [HttpPost]
        public async Task<ActionResult<Evento>> PostEvento(Evento evento)
        {
            // 1. Lógica Inteligente: Si no hay descripción, la IA la genera.
            if (string.IsNullOrWhiteSpace(evento.Descripcion))
            {
                try
                {
                    // Asumimos que evento.Anio y evento.Titulo tienen datos
                    string descripcionIA = await _historiaService.ObtenerExplicacionIA(evento.Anio, evento.Titulo);
                    
                    // Limpieza de Markdown si Gemini lo incluye
                    if (descripcionIA.StartsWith("```"))
                        descripcionIA = descripcionIA.Replace("```", "").Trim();
                    
                    evento.Descripcion = descripcionIA;
                }
                catch (Exception ex)
                {
                    // Si la IA falla, ponemos un mensaje por defecto o logueamos, 
                    // pero permitimos que el evento se cree.
                    evento.Descripcion = "Descripción no disponible en este momento.";
                }
            }

            _context.Eventos.Add(evento);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetEvento", new { id = evento.Id }, evento);
        }

        // DELETE: api/Eventos/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEvento(int id)
        {
            var evento = await _context.Eventos.FindAsync(id);
            if (evento == null)
            {
                return NotFound();
            }

            _context.Eventos.Remove(evento);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        [HttpGet("ia-preview")]
        public async Task<IActionResult> GetIAPreview([FromQuery] int anio, [FromQuery] string titulo)
        {
            if (string.IsNullOrEmpty(titulo)) return BadRequest("El título es obligatorio.");

            var explicacion = await _historiaService.ObtenerExplicacionIA(anio, titulo);

            return Ok(new
            {
                Anio = anio,
                Titulo = titulo,
                SugerenciaDescripcion = explicacion
            });
        }

        private bool EventoExists(int id)
        {
            return _context.Eventos.Any(e => e.Id == id);
        }
    }
}
