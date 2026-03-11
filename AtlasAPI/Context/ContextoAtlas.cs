using Microsoft.EntityFrameworkCore;
using AtlasAPI.Models;
namespace AtlasAPI.Context
{
    public class ContextoAtlas: DbContext
    {

        public ContextoAtlas(DbContextOptions<ContextoAtlas> options) : base(options) { }

        public DbSet<Evento> Eventos { get; set; }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Logro> Logros { get; set; }
        public DbSet<PreguntaQuiz> Preguntas { get; set; }
        public DbSet<UsuarioLogro> UsuariosLogros { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UsuarioLogro>()
                .HasOne(ul => ul.Logro)
                .WithMany()
                .HasForeignKey(ul => ul.LogroId);
        }
    }
}
