using AtlasAPI.Context;
using AtlasAPI.Models;
using AtlasAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace AtlasAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly ContextoAtlas _context;
        private readonly HistoriaService _historiaService;
        public UsuariosController(ContextoAtlas context, HistoriaService historiaService)
        {
            _context = context;
            _historiaService = historiaService;
        }

        // 1. OBTENER TODOS LOS JUGADORES
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Usuario>>> GetUsuarios()
        {
            // Incluimos los logros para verlos en la lista
            return await _context.Usuarios.Include(u => u.Logros).ToListAsync();
        }

        // 2. PERFIL DE UN USUARIO (Para la pantalla de perfil en Android)
        [HttpGet("{id}")]
        public async Task<ActionResult<Usuario>> GetUsuario(int id)
        {
            var usuario = await _context.Usuarios.Include(u => u.Logros)
                                                 .FirstOrDefaultAsync(u => u.Id == id);

            if (usuario == null) return NotFound();
            return usuario;
        }

        // 3. REGISTRO DE NUEVO USUARIO
        [HttpPost]
        public async Task<ActionResult<Usuario>> PostUsuario([FromBody] Usuario usuario)
        {
            // Esto imprimirá en la consola negra de Visual Studio lo que llega de Android
            Console.WriteLine($"REGISTRO RECIBIDO -> Nombre: '{usuario.Nombre}', Pass: '{usuario.Password}'");

            if (string.IsNullOrWhiteSpace(usuario.Nombre) || usuario.Nombre.Length < 4)
            {
                return BadRequest("El nombre de usuario debe tener al menos 4 caracteres.");
            }

            // Validar que no contenga símbolos extraños (Solo letras, números y espacios)
            if (!Regex.IsMatch(usuario.Nombre, @"^[a-zA-Z0-9 ]+$"))
            {
                return BadRequest("El nombre de usuario solo puede contener letras, números y espacios.");
            }

            var existe = await _context.Usuarios.AnyAsync(u => u.Nombre == usuario.Nombre);
            if (existe) return BadRequest("El nombre de usuario ya está registrado");

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();
            return CreatedAtAction("GetUsuario", new { id = usuario.Id }, usuario);
        }

        // 4. ACTUALIZAR PROGRESO (Subir de nivel/experiencia)
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUsuario(int id, Usuario usuario)
        {
            if (id != usuario.Id) return BadRequest();

            _context.Entry(usuario).State = EntityState.Modified;

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

        // 6. BORRAR CUENTA
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUsuario(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null) return NotFound();

            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        //PARA EL LOGIN:

        // 7. LOGIN DE USUARIO (Verificar nombre y contraseña)
        [HttpPost("login")]
        public async Task<ActionResult<Usuario>> Login([FromBody] Usuario loginRequest)
        {
            // Buscamos un usuario que coincida en nombre Y contraseña
            var usuario = await _context.Usuarios
                .Include(u => u.Logros)
                .FirstOrDefaultAsync(u => u.Nombre == loginRequest.Nombre && u.Password == loginRequest.Password);

            if (usuario == null)
            {
                // Si no existe o la contraseña está mal, devolvemos 401 Unauthorized
                return Unauthorized(new { mensaje = "Credenciales incorrectas" });
            }

            // Si todo es correcto, devolvemos el usuario completo para que Android guarde su progreso
            return Ok(usuario);
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
            
            // Devolvemos un objeto compuesto: Usuario + Lista de nuevos logros
            return Ok(new { Usuario = usuario, NuevosLogros = nuevosLogros });
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