using System.Text.RegularExpressions;

namespace AtlasAPI.Helpers
{
    /// <summary>
    /// Métodos de validación de datos reutilizables y testeables de forma aislada.
    /// </summary>
    public static class ValidacionHelper
    {
        /// <summary>
        /// Valida el nombre de usuario.
        /// Reglas: no vacío, mínimo 4 caracteres, solo letras, números y espacios.
        /// </summary>
        public static bool NombreValido(string? nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre)) return false;
            if (nombre.Trim().Length < 4) return false;
            return Regex.IsMatch(nombre, @"^[a-zA-Z0-9 ]+$");
        }

        /// <summary>
        /// Valida la contraseña. Regla: no vacía.
        /// </summary>
        public static bool PasswordValido(string? password)
        {
            return !string.IsNullOrWhiteSpace(password);
        }

        /// <summary>
        /// Valida que el UsuarioId de una partida sea un valor positivo válido.
        /// </summary>
        public static bool UsuarioIdValido(int usuarioId)
        {
            return usuarioId > 0;
        }

        /// <summary>
        /// Valida que el resultado de una partida sea coherente.
        /// Los aciertos deben estar entre 0 y el total de preguntas.
        /// </summary>
        public static bool ResultadoPartidaValido(int aciertos, int totalPreguntas)
        {
            return totalPreguntas > 0 && aciertos >= 0 && aciertos <= totalPreguntas;
        }

        /// <summary>
        /// Calcula el número de siglo a partir de un año.
        /// Años negativos = a.C., positivos = d.C.
        /// Ejemplos: -206 -> -3 (Siglo III a.C.), 929 -> 10 (Siglo X d.C.)
        /// </summary>
        public static int CalcularSiglo(int anio)
        {
            if (anio == 0) return 0;
            int abs = Math.Abs(anio);
            int siglo = (int)Math.Ceiling(abs / 100.0);
            return anio < 0 ? -siglo : siglo;
        }
    }
}
