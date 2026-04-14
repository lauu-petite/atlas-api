using Mscc.GenerativeAI;

namespace AtlasAPI.Services
{
    public class HistoriaService
    {
        private readonly GoogleAI _googleAI;

        public HistoriaService(IConfiguration configuration)
        {
            var apiKey = configuration["Gemini:ApiKey"];
            _googleAI = new GoogleAI(apiKey);
        }

        public async Task<string> GenerarTexto(string prompt)
        {
            // Usamos el ID del modelo directamente como texto. 
            // Esto evita el error de "Model no encontrado".
            var model = _googleAI.GenerativeModel("gemini-1.5-flash");

            var response = await model.GenerateContent(prompt);

            // Devolvemos el texto de la respuesta
            return response.Text;
        }

        public async Task<string> GenerarPreguntaAleatoria(int siglo, string categoria)
        {
            string prompt = $@"
                Genera una pregunta de trivia histórica para una app llamada Atlas.
                Siglo: {siglo}
                Categoría: {categoria}
                
                Responde ÚNICAMENTE con un objeto JSON en este formato exacto:
                {{
                    ""Enunciado"": ""¿Texto de la pregunta?"",
                    ""OpcionA"": ""Opción incorrecta 1"",
                    ""OpcionB"": ""Opción incorrecta 2"",
                    ""OpcionC"": ""Opción correcta"",
                    ""RespuestaCorrecta"": ""Opción correcta"",
                    ""Explicacion"": ""Breve explicación fascinante (máx 2 líneas)"",
                    ""Tema"": ""Nombre del evento"",
                    ""Siglo"": {siglo},
                    ""Categoria"": ""{categoria}""
                }}
                La respuesta correcta debe estar siempre en OpcionC.
            ";

            return await GenerarTexto(prompt);
        }

        public async Task<string> ObtenerExplicacionIA(int anio, string tema)
        {
            // Creamos un prompt estructurado para que la respuesta sea profesional
            string prompt = $@"
                Actúa como un historiador experto. 
                Proporciona una descripción breve (máximo 3 párrafos) y precisa sobre el siguiente evento:
                Evento: {tema}
                Año: {anio}
                
                Céntrate en por qué fue importante y cuáles fueron sus consecuencias, siendo original. 
                Evita introducciones innecesarias como 'Claro, aquí tienes la información'.";

            try
            {
                return await GenerarTexto(prompt);
            }
            catch (Exception ex)
            {
                // Loguear el error si fuera necesario
                return $"Error al generar la descripción: {ex.Message}";
            }
        }
    }
}