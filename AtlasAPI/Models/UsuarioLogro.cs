using System.Text.Json.Serialization;

namespace AtlasAPI.Models
{
    public class UsuarioLogro
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public int LogroId { get; set; }
        public DateTime FechaObtenido { get; set; } = DateTime.Now;

        // Propiedades de navegación para EF Core
        [JsonIgnore]
        public Usuario? Usuario { get; set; }
        public Logro? Logro { get; set; }
    }
}