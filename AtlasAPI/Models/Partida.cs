using System;

namespace AtlasAPI.Models
{
    public class Partida
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public int Aciertos { get; set; }
        public int TotalPreguntas { get; set; }
        public DateTime Fecha { get; set; } = DateTime.Now;

        // Relación detallada de respuestas
        public ICollection<RespuestaPartida> RespuestasPartida { get; set; } = new List<RespuestaPartida>();
    }
}