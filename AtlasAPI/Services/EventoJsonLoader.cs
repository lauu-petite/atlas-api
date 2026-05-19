using System.Text.Json;
using AtlasAPI.Context;
using AtlasAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace AtlasAPI.Services
{
    public class EventoJsonLoader
    {
        private readonly ContextoAtlas _context;

        public EventoJsonLoader(ContextoAtlas context)
        {
            _context = context;
        }

        public async Task LoadAsync()
        {
            try
            {
                if (await _context.Eventos.AnyAsync())
                {
                    Console.WriteLine("⏩ Tabla Eventos ya contiene datos. Saltando carga inicial.");
                    return;
                }

                Console.WriteLine("🛠️ La tabla de eventos está vacía. Lista para recibir datos manuales.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ ERROR TOTAL: {ex.Message}");
            }
        }
    }
}
