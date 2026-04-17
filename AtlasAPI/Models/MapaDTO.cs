using System.Collections.Generic;

namespace AtlasAPI.Models
{
    public class MapaDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public int AnioReferencia { get; set; }
        public string UrlGeoJson { get; set; } = string.Empty;
        public string Leyenda { get; set; } = string.Empty;

        // Lista de eventos relacionados
        public List<Evento> Eventos { get; set; } = new List<Evento>();
    }
}