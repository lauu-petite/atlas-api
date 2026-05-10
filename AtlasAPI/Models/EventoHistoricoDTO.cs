namespace AtlasAPI.Models
{
    public class EventoHistoricoDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public double Lat { get; set; }
        public double Lon { get; set; }
        public string Descripcion { get; set; } = string.Empty;
        public string CategoriaNombre { get; set; } = string.Empty;
        public string CategoriaColor { get; set; } = string.Empty;
        public string ImagenEvento { get; set; } = string.Empty;
        public string Periodo { get; set; } = string.Empty;
    }
}
