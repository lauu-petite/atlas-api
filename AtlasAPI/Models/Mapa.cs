using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AtlasAPI.Models
{
    public class Mapa
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public int AnioReferencia { get; set; }
        public string UrlGeoJson { get; set; } = string.Empty;
        
        [Column(TypeName = "jsonb")]
        public string Leyenda { get; set; } = string.Empty; // Guardará JSON con colores e iconos

        // Relación 1-N: Un mapa tiene muchos eventos
        public ICollection<Evento> Eventos { get; set; } = new List<Evento>();
    }
}