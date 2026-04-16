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

        // Relación 1-N: Un evento pertenece a una categoría
        public int CategoriaEventoId { get; set; }
        public CategoriaEvento? Categoria { get; set; }

        // Relación 1-N: Un evento pertenece a un mapa
        public int MapaId { get; set; }
        public Mapa? Mapa { get; set; }

        // Relación N-N: Usuarios que han marcado este evento como favorito
        public ICollection<UsuarioEventoFavorito> UsuariosFavoritos { get; set; } = new List<UsuarioEventoFavorito>();
    }
}