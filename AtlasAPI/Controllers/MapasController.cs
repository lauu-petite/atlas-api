using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AtlasAPI.Context;
using AtlasAPI.Models;

namespace AtlasAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MapasController : ControllerBase
    {
        private readonly ContextoAtlas _context;

        public MapasController(ContextoAtlas context)
        {
            _context = context;
        }

        [HttpGet("by-year/{anio}")]
        public async Task<ActionResult<Mapa>> GetMapaByAnio(int anio)
        {
            // Logic to find the map that covers the given year
            var mapa = await _context.Mapas
                .Where(m => anio >= m.AnioInicio && anio <= m.AnioFin)
                .FirstOrDefaultAsync();

            if (mapa == null)
            {
                return NotFound();
            }

            return mapa;
        }
    }
}
