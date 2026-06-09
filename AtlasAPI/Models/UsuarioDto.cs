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
        public List<int> EventosFavoritosIds { get; set; } = new();
        public bool EstaBaneado { get; set; } = false;
        public string? Token { get; set; }
    }
}
