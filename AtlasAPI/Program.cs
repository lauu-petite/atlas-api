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
        // Aplicar migraciones automáticamente al arrancar
        Console.WriteLine("Verificando y aplicando migraciones...");
        await context.Database.MigrateAsync();
        Console.WriteLine("Migraciones al día.");

        // Crear mapa base si no existe
        if (!await context.Mapas.AnyAsync())
        {
            context.Mapas.Add(new Mapa { Id = 1, Nombre = "Hispania", Descripcion = "Mapa base de la península" });
            await context.SaveChangesAsync();
            Console.WriteLine("Mapa por defecto creado.");
        }

        // Cargar eventos si está vacía
        if (!await context.Eventos.AnyAsync())
        {
            var loader = services.GetRequiredService<EventoJsonLoader>();
            await loader.LoadAsync();
            Console.WriteLine("Eventos cargados desde archivo JSON.");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine("Aviso en inicialización: " + ex.Message);
    }
}

app.UseAuthorization();
app.MapControllers();

// 5. Configuración dinámica del puerto para Render
var port = Environment.GetEnvironmentVariable("PORT") ?? "5223";
Console.WriteLine("Iniciando Atlas API en el puerto: " + port);

app.Run("http://0.0.0.0:" + port);