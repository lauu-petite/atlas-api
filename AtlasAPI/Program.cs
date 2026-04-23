using AtlasAPI.Context;
using AtlasAPI.Services;
using AtlasAPI.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// 1. Extraer la cadena de conexión del JSON
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// 2. Configurar el contexto para usar MySQL con esa cadena

builder.Services.AddDbContext<ContextoAtlas>(options =>
    options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 30))));
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<HistoriaService>();
builder.Services.AddScoped<EventoJsonLoader>();
var app = builder.Build();

// Sembrar datos desde JSON si es necesario
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ContextoAtlas>();
    
    try
    {
        // Forzar creación de tablas manualmente si EnsureCreated falla
        var connection = context.Database.GetDbConnection();
        await connection.OpenAsync();
        
        using (var command = connection.CreateCommand())
        {
            // 1. Crear tabla Mapas
            command.CommandText = @"
                CREATE TABLE IF NOT EXISTS Mapas (
                    Id INT PRIMARY KEY,
                    Nombre LONGTEXT,
                    Descripcion LONGTEXT,
                    AnioReferencia INT DEFAULT 0,
                    UrlGeoJson LONGTEXT,
                    Leyenda LONGTEXT
                );";
            await command.ExecuteNonQueryAsync();

            // 2. Crear tabla Eventos con todas las columnas necesarias
            command.CommandText = @"
                CREATE TABLE IF NOT EXISTS Eventos (
                    Id INT PRIMARY KEY AUTO_INCREMENT,
                    Anio INT,
                    Titulo LONGTEXT,
                    Descripcion LONGTEXT,
                    Tipo LONGTEXT,
                    Latitud DOUBLE,
                    Longitud DOUBLE,
                    CategoriaNombre LONGTEXT,
                    CategoriaColor LONGTEXT,
                    CategoriaIconoUrl LONGTEXT,
                    MapaId INT,
                    CONSTRAINT FK_Eventos_Mapas FOREIGN KEY (MapaId) REFERENCES Mapas(Id) ON DELETE CASCADE
                );";
            await command.ExecuteNonQueryAsync();
            Console.WriteLine("✅ Tablas verificadas/creadas correctamente.");
        }

        // Asegurar que existe al menos un mapa
        if (!context.Mapas.Any())
        {
            context.Mapas.Add(new Mapa { Id = 1, Nombre = "Hispania", Descripcion = "Mapa base de la península" });
            await context.SaveChangesAsync();
            Console.WriteLine("✅ Mapa por defecto creado.");
        }

        // Limpiar y cargar eventos
        context.Eventos.RemoveRange(context.Eventos);
        await context.SaveChangesAsync();
        
        var loader = services.GetRequiredService<EventoJsonLoader>();
        await loader.LoadAsync();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"⚠️ Error al sincronizar base de datos: {ex.Message}");
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

Console.WriteLine("🚀 Iniciando Atlas API...");
Console.WriteLine("🔗 Swagger disponible en: http://localhost:5223/swagger");

app.Run("http://0.0.0.0:5223");