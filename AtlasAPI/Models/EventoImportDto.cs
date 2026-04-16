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
        public int CategoriaEventoId { get; set; }
        public int MapaId { get; set; }
    }
}