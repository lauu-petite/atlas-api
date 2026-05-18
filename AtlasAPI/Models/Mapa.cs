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

        // Rango de años del mapa conceptual
        public int AnioInicio { get; set; }
        public int AnioFin { get; set; }

        // Identificador del archivo local en Android (ej: "hispania206")
        public string ArchivoHtml { get; set; } = string.Empty;

        [Column(TypeName = "jsonb")]
        public string Leyenda { get; set; } = string.Empty; // Array JSON de colores y nombres

        // Relación 1-N: Un mapa tiene muchos eventos
        public ICollection<Evento> Eventos { get; set; } = new List<Evento>();
    }
}