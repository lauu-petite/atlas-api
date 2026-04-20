using AtlasAPI.Context;
using AtlasAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AtlasAPI.Controllers
{
    [Route("api/eventos")]
    [ApiController]
    public class EventoHistoricoController : ControllerBase
    {
        private readonly ContextoAtlas _context;

        public EventoHistoricoController(ContextoAtlas context)
        {
            _context = context;
        }

        // GET: api/eventos/{anio}
        [HttpGet("{anio}")]
        public async Task<ActionResult<IEnumerable<EventoHistoricoDTO>>> GetEventosPorAnio(int anio)
        {
            var eventos = await _context.Eventos
                .Where(e => e.Anio == anio)
                .ToListAsync();

            if (eventos == null || !eventos.Any())
            {
                return Ok(new List<EventoHistoricoDTO>());
            }

            var dtos = eventos.Select(e => new EventoHistoricoDTO
            {
                Id = e.Id,
                Nombre = e.Titulo,
                Lat = e.Latitud,
                Lon = e.Longitud,
                Descripcion = e.Descripcion,
                CategoriaNombre = e.CategoriaNombre,
                CategoriaColor = e.CategoriaColor,
                CategoriaIconoUrl = e.CategoriaIconoUrl
            }).ToList();

            return Ok(dtos);
        }
    }
}
