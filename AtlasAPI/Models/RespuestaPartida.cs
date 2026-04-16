using System.Text.Json.Serialization;

namespace AtlasAPI.Models
{
    public class RespuestaPartida
    {
        public int PartidaId { get; set; }
        [JsonIgnore]
        public Partida? Partida { get; set; }

        public int PreguntaId { get; set; }
        [JsonIgnore]
        public PreguntaQuiz? Pregunta { get; set; }

        public string RespuestaSeleccionada { get; set; } = string.Empty; // Respuesta que eligió el usuario (A, B, C)
        public bool EsAcertada { get; set; }
    }
}