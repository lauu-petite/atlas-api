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
                    Tipo = "Sistema",
                    Latitud = 40.41,
                    Longitud = -3.70,
                    CategoriaNombre = "Política",
                    CategoriaColor = "#27ae60",
                    CategoriaIconoUrl = "✅",
                    MapaId = 1
                };
                _context.Eventos.Add(prueba);
                await _context.SaveChangesAsync();
                Console.WriteLine("📌 Evento de prueba creado en la base de datos.");

                // 3. CARGAR JSON
                string path = @"C:\Users\desarrollo\source\repos\AtlasAPI\AtlasAPI\Data\eventos.json";
                if (File.Exists(path)) {
                    var json = await File.ReadAllTextAsync(path);
                    var dtos = JsonSerializer.Deserialize<List<EventoImportDto>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    
                    if (dtos != null && dtos.Any()) {
                        var nuevos = dtos.Select(d => new Evento {
                            Anio = d.Anio,
                            Titulo = d.Titulo,
                            Descripcion = d.Descripcion,
                            Tipo = d.Tipo,
                            Latitud = d.Latitud,
                            Longitud = d.Longitud,
                            CategoriaNombre = d.CategoriaNombre,
                            CategoriaColor = d.CategoriaColor,
                            CategoriaIconoUrl = d.CategoriaIconoUrl,
                            MapaId = d.MapaId
                        }).ToList();
                        
                        _context.Eventos.AddRange(nuevos);
                        await _context.SaveChangesAsync();
                        Console.WriteLine($"✅ ¡SÍ! Se han cargado {nuevos.Count} eventos adicionales desde el JSON.");
                    }
                } else {
                    Console.WriteLine($"❌ No se encontró el JSON en {path}, pero el evento de prueba debería estar listo.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ ERROR TOTAL: {ex.Message}");
            }
        }
    }
}
