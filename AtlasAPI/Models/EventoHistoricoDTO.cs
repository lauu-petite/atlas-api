namespace AtlasAPI.Models
{
    public class EventoHistoricoDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public double Lat { get; set; }
        public double Lon { get; set; }
        public string Descripcion { get; set; } = string.Empty;
        public int CategoriaId { get; set; } // 1: Política, 2: Ciencia, 3: Guerra, 4: Arte
    }
}
