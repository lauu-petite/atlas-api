using Microsoft.EntityFrameworkCore;
using AtlasAPI.Models;

namespace AtlasAPI.Context
{
    public class ContextoAtlas : DbContext
    {

        public ContextoAtlas(DbContextOptions<ContextoAtlas> options) : base(options) { }

        public DbSet<Evento> Eventos { get; set; }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Logro> Logros { get; set; }
        public DbSet<PreguntaQuiz> Preguntas { get; set; }
        public DbSet<UsuarioLogro> UsuariosLogros { get; set; }
        public DbSet<Partida> Partidas { get; set; }
        public DbSet<Mapa> Mapas { get; set; }
        public DbSet<RespuestaPartida> RespuestasPartida { get; set; }
        public DbSet<UsuarioEventoFavorito> UsuarioEventoFavoritos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // UsuarioLogro
            modelBuilder.Entity<UsuarioLogro>()
                .HasOne(ul => ul.Logro)
                .WithMany()
                .HasForeignKey(ul => ul.LogroId);

            // Relación N-N: Partida - PreguntaQuiz (RespuestaPartida)
            modelBuilder.Entity<RespuestaPartida>()
                .HasKey(rp => new { rp.PartidaId, rp.PreguntaId });

            modelBuilder.Entity<RespuestaPartida>()
                .HasOne(rp => rp.Partida)
                .WithMany(p => p.RespuestasPartida)
                .HasForeignKey(rp => rp.PartidaId);

            modelBuilder.Entity<RespuestaPartida>()
                .HasOne(rp => rp.Pregunta)
                .WithMany(q => q.RespuestasPartida)
                .HasForeignKey(rp => rp.PreguntaId);

            // Relación N-N: Usuario - Evento (Favoritos)
            modelBuilder.Entity<UsuarioEventoFavorito>()
                .HasKey(uef => new { uef.UsuarioId, uef.EventoId });

            modelBuilder.Entity<UsuarioEventoFavorito>()
                .HasOne(uef => uef.Usuario)
                .WithMany(u => u.EventosFavoritos)
                .HasForeignKey(uef => uef.UsuarioId);

            // Relación N-N: Usuario - Evento (Favoritos)
            modelBuilder.Entity<UsuarioEventoFavorito>()
                .HasOne(uef => uef.Evento)
                .WithMany(e => e.UsuariosFavoritos)
                .HasForeignKey(uef => uef.EventoId);

            // Relación 1-N: Mapa - Eventos
            modelBuilder.Entity<Mapa>()
                .HasMany(m => m.Eventos)
                .WithOne(e => e.Mapa)
                .HasForeignKey(e => e.MapaId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
