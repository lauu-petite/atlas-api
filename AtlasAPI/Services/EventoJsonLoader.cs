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
                Console.WriteLine("🛠️ Iniciando carga forzada de eventos...");

                // 1. LIMPIEZA TOTAL
                await _context.Database.ExecuteSqlRawAsync("SET FOREIGN_KEY_CHECKS = 0;");
                await _context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE Eventos;");
                await _context.Database.ExecuteSqlRawAsync("SET FOREIGN_KEY_CHECKS = 1;");

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

                // 3. CARGAR JSON (FUNCIONALIDAD ELIMINADA POR REQUISITO DEL USUARIO)
                Console.WriteLine("Funcionalidad de carga de eventos desde JSON eliminada por requisito del usuario.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ ERROR TOTAL: {ex.Message}");
            }
        }
    }
}
