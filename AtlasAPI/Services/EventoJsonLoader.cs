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

                Console.WriteLine("🛠️ Cargando evento de prueba (tabla vacía)...");

                // 2. EVENTO DE PRUEBA (Para confirmar que la DB funciona)
                var prueba = new Evento {
                    Anio = 2026,
                    Titulo = "Conexión Establecida",
                    Descripcion = "Si ves esto, la base de datos está funcionando correctamente.",
                    Latitud = 40.41,
                    Longitud = -3.70,
                    CategoriaNombre = "Política",
                    CategoriaColor = "#27ae60",
                    ImagenEvento = "https://example.com/image.png",
                    MapaId = 1
                };
                _context.Eventos.Add(prueba);
                await _context.SaveChangesAsync();
                Console.WriteLine("📌 Evento de prueba creado en la base de datos.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ ERROR TOTAL: {ex.Message}");
            }
        }
    }
}
