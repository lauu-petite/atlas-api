namespace AtlasAPI.Models
{
    public class EventoImportDto
    {
        public int Id { get; set; }
        public int Anio { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty;
        public double Latitud { get; set; }
        public double Longitud { get; set; }
        public string CategoriaNombre { get; set; } = string.Empty;
        public string CategoriaColor { get; set; } = string.Empty;
        public string ImagenEvento { get; set; } = string.Empty;
        public int Periodo { get; set; }
        public int MapaId { get; set; }
    }
}