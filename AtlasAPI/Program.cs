using AtlasAPI.Context;
using AtlasAPI.Services;
using AtlasAPI.Models;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;

var builder = WebApplication.CreateBuilder(args);

// 1. Extraer la cadena de conexión
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// 2. Configurar el contexto con versión fija para evitar errores de conexión al arrancar
builder.Services.AddDbContext<ContextoAtlas>(options =>
    options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 30)),
        mysqlOptions => mysqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(10),
            errorNumbersToAdd: null)));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<HistoriaService>();
builder.Services.AddScoped<EventoJsonLoader>();

var app = builder.Build();

// 3. Configuración de Swagger fuera de bloques condicionales para Render
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Atlas API V1");
    c.RoutePrefix = string.Empty; // Swagger aparecerá en la raíz de la URL
});

// 4. Lógica de inicialización de base de datos profesional
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ContextoAtlas>();

    try
    {
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

            // 2. Crear tabla Eventos
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
                    ImagenEvento LONGTEXT,
                    Periodo LONGTEXT,
                    MapaId INT,
                    CONSTRAINT FK_Eventos_Mapas FOREIGN KEY (MapaId) REFERENCES Mapas(Id) ON DELETE CASCADE
                );";
            await command.ExecuteNonQueryAsync();

            // MIGRACIÓN MANUAL: Si existe CategoriaIconoUrl, renombrar/cambiar campos
            try {
                command.CommandText = "ALTER TABLE Eventos DROP COLUMN CategoriaIconoUrl;";
                await command.ExecuteNonQueryAsync();
                
                command.CommandText = "ALTER TABLE Eventos ADD COLUMN ImagenEvento LONGTEXT, ADD COLUMN Periodo LONGTEXT;";
                await command.ExecuteNonQueryAsync();
                Console.WriteLine("✅ Tabla Eventos migrada correctamente (eliminado CategoriaIconoUrl, añadidos ImagenEvento y Periodo).");
            } catch { /* Ignorar si ya se hizo o si falla por no existir la columna vieja */ }

            // 3. NUEVO: Crear tabla Usuarios (La que faltaba)
            command.CommandText = @"
                CREATE TABLE IF NOT EXISTS Usuarios (
                    Id INT PRIMARY KEY AUTO_INCREMENT,
                    Nombre LONGTEXT,
                    Email LONGTEXT,
                    Password LONGTEXT,
                    FechaRegistro DATETIME DEFAULT CURRENT_TIMESTAMP
                );";
            await command.ExecuteNonQueryAsync();

            Console.WriteLine("✅ Todas las tablas (incluyendo Usuarios) verificadas correctamente.");
        }

        // Asegurar que existe al menos un mapa
        if (!context.Mapas.Any())
        {
            context.Mapas.Add(new Mapa { Id = 1, Nombre = "Hispania", Descripcion = "Mapa base de la península" });
            await context.SaveChangesAsync();
            Console.WriteLine("Mapa por defecto creado.");
        }

        // Limpiar y cargar eventos
        context.Eventos.RemoveRange(context.Eventos);
        await context.SaveChangesAsync();

        var loader = services.GetRequiredService<EventoJsonLoader>();
        await loader.LoadAsync();
    }
    catch (Exception ex)
    {
        Console.WriteLine("Aviso en inicialización: " + ex.Message);
    }
}

// Swagger habilitado siempre para que no de problemas en Docker
app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthorization();
app.MapControllers();

Console.WriteLine("🚀 Iniciando Atlas API...");
Console.WriteLine("🔗 Puerto interno: 5223");

app.Run("http://0.0.0.0:5223");
