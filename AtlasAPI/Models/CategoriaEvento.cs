using System.Collections.Generic;

namespace AtlasAPI.Models
{
    public class CategoriaEvento
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty; // Guerra, Política, Arte...
        public string Color { get; set; } = "#FF0000"; // Color hexadecimal para el mapa
        public string IconoUrl { get; set; } = string.Empty; // URL o nombre del icono

        // Relación 1-N: Una categoría tiene muchos eventos
        public ICollection<Evento> Eventos { get; set; } = new List<Evento>();

        // Relación N-N: Categorías disponibles en un mapa
        public ICollection<Mapa> Mapas { get; set; } = new List<Mapa>();
    }
}