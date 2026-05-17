namespace AtlasAPI.Models
{
    public class UsuarioDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Rol { get; set; } = "USER";
        public int Nivel { get; set; } = 1;
        public int Experiencia { get; set; } = 0;
        public string Avatar { get; set; } = "isabel";
        public List<Logro> Logros { get; set; } = new();
        public List<int> EventosFavoritosIds { get; set; } = new();
        public string? Token { get; set; }
    }
}
