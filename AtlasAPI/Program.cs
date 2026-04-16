using AtlasAPI.Context;
using AtlasAPI.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// 1. Extraer la cadena de conexión del JSON
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// 2. Configurar el contexto para usar MySQL con esa cadena
builder.Services.AddDbContext<ContextoAtlas>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));
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
    try
    {
        var loader = scope.ServiceProvider.GetRequiredService<EventoJsonLoader>();
        await loader.LoadAsync();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"⚠️ Error al cargar datos iniciales: {ex.Message}");
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

app.Run();
