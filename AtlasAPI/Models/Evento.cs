namespace AtlasAPI.Models
{
    public class Evento
    {
        public int Id { get; set; }
        public int Anio { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty; // Ejemplo: "Guerra", "Cultura", "Ciencia"
        public double Latitud { get; set; }
        public double Longitud { get; set; }

        // Propiedades de categoría "aplanadas" para compatibilidad con Android
        public string CategoriaNombre { get; set; } = string.Empty;
        public string CategoriaColor { get; set; } = string.Empty;
        public string ImagenEvento { get; set; } = string.Empty;
        public string Periodo { get; set; } = string.Empty;

        // RRelación 1-N: Un evento pertenece a un mapa
        public int MapaId { get; set; }
        public Mapa? Mapa { get; set; }

        // Relación N-N: Usuarios que han marcado este evento como favorito
        public ICollection<UsuarioEventoFavorito> UsuariosFavoritos { get; set; } = new List<UsuarioEventoFavorito>();
    }
}