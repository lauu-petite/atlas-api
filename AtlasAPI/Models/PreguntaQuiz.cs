using System.Text.Json.Serialization;

namespace AtlasAPI.Models
{
    public class PreguntaQuiz
    {
        public int Id { get; set; }
        public string Enunciado { get; set; } = string.Empty;
        public string OpcionA { get; set; } = string.Empty;
        public string OpcionB { get; set; } = string.Empty;
        public string OpcionC { get; set; } = string.Empty;
        public string RespuestaCorrecta { get; set; } = string.Empty;
        
        public string Explicacion { get; set; } = string.Empty;
        public string Tema { get; set; } = string.Empty; 
        
        // Nuevos campos para filtrado
        public int Siglo { get; set; }
        public string Categoria { get; set; } = string.Empty;

        // Relación con respuestas de partidas
        public ICollection<RespuestaPartida> RespuestasPartida { get; set; } = new List<RespuestaPartida>();
    }
}