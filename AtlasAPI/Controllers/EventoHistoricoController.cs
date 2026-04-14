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
                CategoriaId = MapTipoToCategoriaId(e.Tipo)
            }).ToList();

            return Ok(dtos);
        }

        private static int MapTipoToCategoriaId(string tipo)
        {
            if (string.IsNullOrEmpty(tipo)) return 1;

            return tipo.ToLower().Trim() switch
            {
                "política" or "politica" or "politics" => 1,
                "ciencia" or "science" or "tecnología" or "tecnologia" => 2,
                "guerra" or "war" or "militar" or "conflicto" => 3,
                "arte" or "art" or "cultura" or "culture" => 4,
                _ => 1 // Por defecto Política si no coincide
            };
        }
    }
}
