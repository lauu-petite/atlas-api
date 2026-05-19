using AtlasAPI.Context;
using AtlasAPI.Services;
using AtlasAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using MySqlConnector;

var builder = WebApplication.CreateBuilder(args);

// CONFIGURACIÓN DE BASE DE DATOS (MySQL Aiven)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ContextoAtlas>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString),
        mySqlOptions => mySqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(10),
            errorNumbersToAdd: null)));

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Inyectar Servicios
builder.Services.AddScoped<EventoJsonLoader>();
builder.Services.AddScoped<HistoriaService>();

// CONFIGURACIÓN DE JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdmin", policy => policy.RequireRole("ADMIN"));
});

var app = builder.Build();

// --- INICIALIZACIÓN DE DATOS (SEEDING) ---
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ContextoAtlas>();
        
        // 1. APLICAR ESTRUCTURA
        // Intentamos migraciones primero
        try {
            await context.Database.MigrateAsync();
            Console.WriteLine("✅ Intento de MigrateAsync completado.");
        } catch (Exception migEx) {
            Console.WriteLine($"⚠️ MigrateAsync falló: {migEx.Message}. Intentando EnsureCreated...");
            await context.Database.EnsureCreatedAsync(); 
        }
        
        // Verificación extra: ¿Se han creado las tablas?
        var canConnect = await context.Database.CanConnectAsync();
        Console.WriteLine($"DB Conectada: {canConnect}");

        // 2. SEEDING: Asegurar que existe el usuario admin
        var adminUser = await context.Usuarios.FirstOrDefaultAsync(u => u.Nombre == "admin");
        if (adminUser == null)
        {
            var hashedPw = BCrypt.Net.BCrypt.HashPassword("admin789");
            context.Usuarios.Add(new Usuario 
            { 
                Nombre = "admin", 
                PasswordHash = hashedPw, 
                Rol = "ADMIN",
                Avatar = "isabel",
                Nivel = 100,
                Experiencia = 0,
                EstaBaneado = false
            });
            await context.SaveChangesAsync();
            Console.WriteLine("👤 Usuario 'admin' creado correctamente.");
        }
        else 
        {
            // Asegurar hash correcto para admin
            adminUser.PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin789");
            adminUser.Rol = "ADMIN";
            await context.SaveChangesAsync();
            Console.WriteLine("👤 Usuario 'admin' verificado.");
        }

        // 3. SEEDING: Asegurar que existe al menos un mapa
        if (!context.Mapas.Any())
        {
            context.Mapas.Add(new Mapa { Id = 1, Nombre = "Hispania", Descripcion = "Mapa base de la península" });
            await context.SaveChangesAsync();
            Console.WriteLine("🗺️ Mapa por defecto creado.");
        }

        // 4. CARGAR EVENTOS INICIALES (Si la tabla está vacía)
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
