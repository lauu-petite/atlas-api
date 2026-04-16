using System.Text.Json.Serialization;

namespace AtlasAPI.Models
{
    public class UsuarioEventoFavorito
    {
        public int UsuarioId { get; set; }
        [JsonIgnore]
        public Usuario? Usuario { get; set; }

        public int EventoId { get; set; }
        [JsonIgnore]
        public Evento? Evento { get; set; }
        
        public DateTime FechaGuardado { get; set; } = DateTime.Now;
    }
}