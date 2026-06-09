using AtlasAPI.Helpers;
using Xunit;

namespace AtlasAPI.Tests
{
    /// <summary>
    /// Pruebas unitarias para ValidacionHelper.
    /// Se ejecutan en local sin base de datos ni conexión a red.
    /// </summary>
    public class ValidacionHelperTests
    {
        // ── NombreValido ────────────────────────────────────────────────────────

        [Fact]
        public void Nombre_ValidoCuatroCaracteres()
        {
            Assert.True(ValidacionHelper.NombreValido("Juan"));
        }

        [Fact]
        public void Nombre_ValidoConEspacio()
        {
            Assert.True(ValidacionHelper.NombreValido("Juan Garcia"));
        }

        [Fact]
        public void Nombre_ValidoConNumeros()
        {
            Assert.True(ValidacionHelper.NombreValido("Atlas123"));
        }

        [Fact]
        public void Nombre_RechazadoVacio()
        {
            Assert.False(ValidacionHelper.NombreValido(""));
        }

        [Fact]
        public void Nombre_RechazadoNull()
        {
            Assert.False(ValidacionHelper.NombreValido(null));
        }

        [Fact]
        public void Nombre_RechazadoMenosDeCuatroCaracteres()
        {
            Assert.False(ValidacionHelper.NombreValido("Ana"));
        }

        [Fact]
        public void Nombre_RechazadoConArroba()
        {
            Assert.False(ValidacionHelper.NombreValido("Juan@Atlas"));
        }

        [Fact]
        public void Nombre_RechazadoConGuion()
        {
            Assert.False(ValidacionHelper.NombreValido("Ana-Maria"));
        }

        [Fact]
        public void Nombre_RechazadoSoloEspacios()
        {
            Assert.False(ValidacionHelper.NombreValido("    "));
        }

        // ── PasswordValido ───────────────────────────────────────────────────────

        [Fact]
        public void Password_ValidoNormal()
        {
            Assert.True(ValidacionHelper.PasswordValido("miclave123"));
        }

        [Fact]
        public void Password_RechazadoVacio()
        {
            Assert.False(ValidacionHelper.PasswordValido(""));
        }

        [Fact]
        public void Password_RechazadoNull()
        {
            Assert.False(ValidacionHelper.PasswordValido(null));
        }

        [Fact]
        public void Password_RechazadoSoloEspacios()
        {
            Assert.False(ValidacionHelper.PasswordValido("   "));
        }

        // ── UsuarioIdValido ──────────────────────────────────────────────────────

        [Fact]
        public void UsuarioId_ValidoPositivo()
        {
            Assert.True(ValidacionHelper.UsuarioIdValido(1));
        }

        [Fact]
        public void UsuarioId_RechazadoCero()
        {
            Assert.False(ValidacionHelper.UsuarioIdValido(0));
        }

        [Fact]
        public void UsuarioId_RechazadoNegativo()
        {
            Assert.False(ValidacionHelper.UsuarioIdValido(-5));
        }

        // ── ResultadoPartidaValido ───────────────────────────────────────────────

        [Fact]
        public void Partida_ResultadoValido()
        {
            Assert.True(ValidacionHelper.ResultadoPartidaValido(4, 6));
        }

        [Fact]
        public void Partida_ResultadoCeroAciertos()
        {
            Assert.True(ValidacionHelper.ResultadoPartidaValido(0, 6));
        }

        [Fact]
        public void Partida_ResultadoPerfecto()
        {
            Assert.True(ValidacionHelper.ResultadoPartidaValido(6, 6));
        }

        [Fact]
        public void Partida_RechazadoAciertosMayoresQueTotal()
        {
            Assert.False(ValidacionHelper.ResultadoPartidaValido(7, 6));
        }

        [Fact]
        public void Partida_RechazadoAciertosNegativos()
        {
            Assert.False(ValidacionHelper.ResultadoPartidaValido(-1, 6));
        }

        [Fact]
        public void Partida_RechazadoTotalCero()
        {
            Assert.False(ValidacionHelper.ResultadoPartidaValido(0, 0));
        }

        // ── CalcularSiglo ────────────────────────────────────────────────────────

        [Fact]
        public void Siglo_Anio929EsSigloX()
        {
            Assert.Equal(10, ValidacionHelper.CalcularSiglo(929));
        }

        [Fact]
        public void Siglo_Anio206AntesCristoEsSigloIII()
        {
            Assert.Equal(-3, ValidacionHelper.CalcularSiglo(-206));
        }

        [Fact]
        public void Siglo_Anio1492EsSigloXV()
        {
            Assert.Equal(15, ValidacionHelper.CalcularSiglo(1492));
        }

        [Fact]
        public void Siglo_Anio100EsSigloI()
        {
            Assert.Equal(1, ValidacionHelper.CalcularSiglo(100));
        }

        [Fact]
        public void Siglo_Anio101EsSigloII()
        {
            Assert.Equal(2, ValidacionHelper.CalcularSiglo(101));
        }

        [Fact]
        public void Siglo_AnioNegativo1EsSigloIAntesCristo()
        {
            Assert.Equal(-1, ValidacionHelper.CalcularSiglo(-1));
        }
    }
}
