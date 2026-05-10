using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;

namespace AtlasAPI.Models
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        
        // La contraseña se recibe en texto plano pero NO se guarda en la DB
        [NotMapped]
        public string Password { get; set; } = string.Empty; 
        
        [JsonIgnore]
        public string PasswordHash { get; set; } = string.Empty;
        
        public string Rol { get; set; } = "USER"; // ADMIN o USER
        public bool EstaBaneado { get; set; } = false;

        public int Nivel { get; set; } = 1;
        public int Experiencia { get; set; } = 0;
        public string Avatar { get; set; } = "isabel";
        public List<Logro> Logros { get; set; } = new();

        public ICollection<UsuarioEventoFavorito> EventosFavoritos { get; set; } = new List<UsuarioEventoFavorito>();
    }
}
