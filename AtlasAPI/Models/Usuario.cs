using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;

namespace AtlasAPI.Models
{
    public class Usuario
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public int Nivel { get; set; } = 1; // Nivel del usuario para la IA
        public int Experiencia { get; set; } = 0;
        public string Avatar { get; set; } = "isabel"; // Isabel, Fernando, Seneca...
        public List<Logro> Logros { get; set; } = new();
    }
}
