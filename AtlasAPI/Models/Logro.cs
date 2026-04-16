namespace AtlasAPI.Models
{
    public class Logro
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty; // Ej: "Maestro del Romanticismo"
        public string Descripcion { get; set; } = string.Empty;
        public DateTime FechaObtenido { get; set; } = DateTime.Now;
        public string Icono { get; set; } = string.Empty;
    }
}
