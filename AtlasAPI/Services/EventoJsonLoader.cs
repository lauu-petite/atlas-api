using System.Text.Json;
using AtlasAPI.Context;
using AtlasAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace AtlasAPI.Services
{
    public class EventoJsonLoader
    {
        private readonly ContextoAtlas _context;
        private readonly ILogger<EventoJsonLoader> _logger;

        public EventoJsonLoader(ContextoAtlas context, ILogger<EventoJsonLoader> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task LoadAsync()
        {
            try
            {
                // Ruta robusta combinando ambas posibilidades
                var path = Path.Combine(AppContext.BaseDirectory, "Data", "eventos.json");
                if (!File.Exists(path))
                {
                    path = Path.Combine(Directory.GetCurrentDirectory(), "Data", "eventos.json");
                }

                if (!File.Exists(path))
                {
                    _logger.LogWarning("⚠️ No se encontró el archivo JSON de eventos en: {Path}", path);
                    return;
                }

                var json = await File.ReadAllTextAsync(path);
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var dtos = JsonSerializer.Deserialize<List<EventoImportDto>>(json, options);

                if (dtos == null || !dtos.Any()) return;

                // Obtener IDs ya existentes para evitar duplicados
                var existingIds = await _context.Eventos.Select(e => e.Id).ToListAsync();
                var newEvents = dtos.Where(d => !existingIds.Contains(d.Id)).Select(d => new Evento
                {
                    Id = d.Id,
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

                if (newEvents.Any())
                {
                    // Forzar inserción de ID si la base de datos usa autoincremental
                    using (var transaction = await _context.Database.BeginTransactionAsync())
                    {
                        try {
                            _context.Eventos.AddRange(newEvents);
                            await _context.SaveChangesAsync();
                            await transaction.CommitAsync();
                            _logger.LogInformation("✅ Se han importado {Count} nuevos eventos desde el JSON.", newEvents.Count);
                        } catch (Exception ex) {
                            await transaction.RollbackAsync();
                            _logger.LogError(ex, "❌ Error guardando los eventos del JSON.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Error crítico al cargar el JSON de eventos.");
            }
        }
    }
}