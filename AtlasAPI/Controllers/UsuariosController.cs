using AtlasAPI.Context;
using AtlasAPI.Models;
using AtlasAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;

namespace AtlasAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly ContextoAtlas _context;
        private readonly HistoriaService _historiaService;
        private readonly IConfiguration _configuration;

        public UsuariosController(ContextoAtlas context, HistoriaService historiaService, IConfiguration configuration)
        {
            _context = context;
            _historiaService = historiaService;
            _configuration = configuration;
        }

        // 1. OBTENER TODOS LOS JUGADORES (MODO ADMIN)
        [Authorize(Roles = "ADMIN")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UsuarioDto>>> GetUsuarios([FromHeader(Name = "Admin-Id")] int adminId, [FromHeader(Name = "Admin-Key")] string key)
        {
            // 1. Verificar Key estática
            if (key != "ADMIN") return Unauthorized("Llave de admin incorrecta");

            // 2. Verificar Rol en Base de Datos
            var admin = await _context.Usuarios.FindAsync(adminId);
            if (admin == null || admin.Rol != "ADMIN") 
                return Unauthorized("No tienes permisos de administrador o el ID es inválido");

            var usuarios = await _context.Usuarios.Include(u => u.Logros).ToListAsync();
            return Ok(usuarios.Select(MapToDto));
        }

        // 2. PERFIL DE UN USUARIO
        [HttpGet("{id}")]
        public async Task<ActionResult<UsuarioDto>> GetUsuario(int id)
        {
            var usuario = await _context.Usuarios
                .Include(u => u.Logros)
                .Include(u => u.EventosFavoritos)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (usuario == null) return NotFound();
            return Ok(MapToDto(usuario));
        }

        // 3. REGISTRO DE NUEVO USUARIO (Con Hashing)
        [HttpPost]
        public async Task<ActionResult<UsuarioDto>> PostUsuario([FromBody] Usuario usuario)
        {
            if (string.IsNullOrWhiteSpace(usuario.Nombre) || usuario.Nombre.Length < 4)
            {
                return BadRequest("El nombre de usuario debe tener al menos 4 caracteres.");
            }

            if (string.IsNullOrWhiteSpace(usuario.Password))
            {
                return BadRequest("La contraseña es obligatoria.");
            }

            if (!Regex.IsMatch(usuario.Nombre, @"^[a-zA-Z0-9 ]+$"))
            {
                return BadRequest("El nombre de usuario solo puede contener letras, números y espacios.");
            }

            var existe = await _context.Usuarios.AnyAsync(u => u.Nombre == usuario.Nombre);
            if (existe) return BadRequest("El nombre de usuario ya está registrado");

            // SEGURIDAD: Hash de contraseña y rol por defecto
            usuario.PasswordHash = BCrypt.Net.BCrypt.HashPassword(usuario.Password);
            usuario.Rol = "USER";
            usuario.EstaBaneado = false;

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();
            
            return CreatedAtAction("GetUsuario", new { id = usuario.Id }, MapToDto(usuario));
        }

        // 4. ACTUALIZAR PROGRESO (Subir de nivel/experiencia)
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUsuario(int id, Usuario usuario)
        {
            if (id != usuario.Id) return BadRequest();

            var usuarioEnDb = await _context.Usuarios.FindAsync(id);
            if (usuarioEnDb == null) return NotFound();

            // Solo permitimos actualizar campos de progreso, NO el password ni el rol desde aquí
            usuarioEnDb.Nivel = usuario.Nivel;
            usuarioEnDb.Experiencia = usuario.Experiencia;
            usuarioEnDb.Avatar = usuario.Avatar;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UsuarioExists(id)) return NotFound();
                else throw;
            }

            return NoContent();
        }

        // 5. DAR UN LOGRO A UN USUARIO (Gamificación)
        [HttpPost("{id}/logros")]
        public async Task<IActionResult> AddLogro(int id, Logro nuevoLogro)
        {
            var usuario = await _context.Usuarios.Include(u => u.Logros)
                                                 .FirstOrDefaultAsync(u => u.Id == id);

            if (usuario == null) return NotFound("Usuario no encontrado");

            nuevoLogro.Id = 0;
            usuario.Logros.Add(nuevoLogro);

            await _context.SaveChangesAsync();
            return Ok(nuevoLogro);
        }

        // 6. BORRAR CUENTA (ADMIN)
        [Authorize(Roles = "ADMIN")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUsuario(int id, [FromHeader(Name = "Admin-Id")] int adminId, [FromHeader(Name = "Admin-Key")] string key)
        {
            if (key != "ADMIN") return Unauthorized();
            
            var admin = await _context.Usuarios.FindAsync(adminId);
            if (admin == null || admin.Rol != "ADMIN") return Unauthorized();

            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null) return NotFound();

            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // 7. LOGIN DE USUARIO (Con Verificación de Hash y Ban)
        [HttpPost("login")]
        public async Task<ActionResult<UsuarioDto>> Login([FromBody] Usuario loginRequest)
        {
            // 1. Buscar solo por nombre con favoritos
            var usuario = await _context.Usuarios
                .Include(u => u.Logros)
                .Include(u => u.EventosFavoritos)
                .FirstOrDefaultAsync(u => u.Nombre == loginRequest.Nombre);

            // 2. Verificar si existe y si la contraseña coincide (BCrypt)
            if (usuario == null || !BCrypt.Net.BCrypt.Verify(loginRequest.Password, usuario.PasswordHash))
            {
                return Unauthorized(new { mensaje = "Credenciales incorrectas" });
            }

            // 3. Verificar si está baneado
            if (usuario.EstaBaneado)
            {
                return Unauthorized(new { mensaje = "Tu cuenta ha sido suspendida (Baneado)" });
            }

            // 4. Generar JWT y devolver DTO con token
            var dto = MapToDto(usuario);
            dto.Token = GenerarToken(usuario);
            return Ok(dto);
        }

        private string GenerarToken(Usuario usuario)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiracion = DateTime.UtcNow.AddDays(int.Parse(_configuration["Jwt:ExpirationDays"] ?? "30"));

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, usuario.Id.ToString()),
                new Claim(ClaimTypes.Name, usuario.Nombre),
                new Claim(ClaimTypes.Role, usuario.Rol),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: expiracion,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        // ADMIN: BANEAR
        [Authorize(Roles = "ADMIN")]
        [HttpPut("{id}/ban")]
        public async Task<IActionResult> BanUsuario(int id, [FromHeader(Name = "Admin-Id")] int adminId, [FromHeader(Name = "Admin-Key")] string key)
        {
            if (key != "ADMIN") return Unauthorized();
            
            var admin = await _context.Usuarios.FindAsync(adminId);
            if (admin == null || admin.Rol != "ADMIN") return Unauthorized();
            
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null) return NotFound();

            usuario.EstaBaneado = true;
            await _context.SaveChangesAsync();
            return Ok(new { mensaje = $"Usuario {usuario.Nombre} baneado" });
        }

        // ADMIN: DESBANEAR
        [Authorize(Roles = "ADMIN")]
        [HttpPut("{id}/unban")]
        public async Task<IActionResult> UnbanUsuario(int id, [FromHeader(Name = "Admin-Id")] int adminId, [FromHeader(Name = "Admin-Key")] string key)
        {
            if (key != "ADMIN") return Unauthorized();

            var admin = await _context.Usuarios.FindAsync(adminId);
            if (admin == null || admin.Rol != "ADMIN") return Unauthorized();

            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null) return NotFound();

            usuario.EstaBaneado = false;
            await _context.SaveChangesAsync();
            return Ok(new { mensaje = $"Usuario {usuario.Nombre} activado de nuevo" });
        }

        private UsuarioDto MapToDto(Usuario u)
        {
            return new UsuarioDto
            {
                Id = u.Id,
                Nombre = u.Nombre,
                Rol = u.Rol,
                Nivel = u.Nivel,
                Experiencia = u.Experiencia,
                Avatar = u.Avatar,
                Logros = u.Logros,
                EventosFavoritosIds = u.EventosFavoritos?.Select(f => f.EventoId).ToList() ?? new List<int>()
            };
        }

        [HttpPost("sumarExperiencia/{id}")]
        public async Task<IActionResult> SumarExperiencia(int id, [FromBody] int exp)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null) return NotFound();

            usuario.Experiencia += exp;
            
            if (usuario.Experiencia >= 100)
            {
                usuario.Nivel++;
                usuario.Experiencia -= 100;
            }

            // --- SISTEMA DE LOGROS AUTOMÁTICO CON NOTIFICACIÓN ---
            var todosLosLogros = await _context.Logros.ToListAsync();
            var logrosActuales = await _context.UsuariosLogros
                .Where(ul => ul.UsuarioId == id)
                .Select(ul => ul.LogroId)
                .ToListAsync();

            var nuevosLogros = new List<Logro>();

            foreach (var logro in todosLosLogros)
            {
                // Si cumple el requisito y NO lo tiene ya
                if (!logrosActuales.Contains(logro.Id) && usuario.Nivel >= logro.Id)
                {
                    var nuevoRegistro = new UsuarioLogro { UsuarioId = id, LogroId = logro.Id };
                    _context.UsuariosLogros.Add(nuevoRegistro);
                    nuevosLogros.Add(logro); // Lo guardamos para avisar a Android
                }
            }

            await _context.SaveChangesAsync();
            
            // Devolvemos DTO para no exponer campos sensibles
            return Ok(new { Usuario = MapToDto(usuario), NuevosLogros = nuevosLogros });
        }

        //IA

        [HttpGet("{id}/saludo")]
        public async Task<ActionResult<object>> GetSaludoIA(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null) return NotFound();

            // Si el usuario no tiene avatar, usamos Isabel por defecto
            string personaje = string.IsNullOrEmpty(usuario.Avatar) ? "Isabel la Católica" : usuario.Avatar;

            // Creamos el prompt para Gemini
            string prompt = $"Eres el personaje histórico {personaje}. Saluda al usuario '{usuario.Nombre}' de mi app de historia 'Atlas' con una frase muy corta (máximo 15 palabras) que refleje tu personalidad. Sé original y encantador .";

            try
            {
                string saludo = await _historiaService.GenerarTexto(prompt);
                return Ok(new { saludo = saludo.Replace("\"", "") }); // Limpia comillas extra
            }
            catch (Exception ex)
            {
                return Ok(new { saludo = "La historia te espera, explorador..." }); // Saludo de emergencia si falla la IA
            }
        }
        private bool UsuarioExists(int id)
        {
            return _context.Usuarios.Any(e => e.Id == id);
        }
    }
}