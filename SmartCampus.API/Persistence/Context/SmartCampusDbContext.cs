using Microsoft.EntityFrameworkCore;
using SmartCampus.API.Domain.Entities;

namespace SmartCampus.API.Persistence.Context
{
    public class SmartCampusDbContext : DbContext
    {
        public SmartCampusDbContext(
            DbContextOptions<SmartCampusDbContext> options)
            : base(options) { }

        public DbSet<Rol> Roles { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Estudiante> Estudiantes { get; set; }
        public DbSet<PersonalAdministrativo> PersonalAdministrativo { get; set; }
        public DbSet<Tramite> Tramites { get; set; }
        public DbSet<HistorialTramite> HistorialTramites { get; set; }
        public DbSet<Ventanilla> Ventanillas { get; set; }
        public DbSet<Turno> Turnos { get; set; }
        public DbSet<AccionUsuario> AccionesUsuario { get; set; }
        public DbSet<CategoriaDoc> CategoriasDoc { get; set; }
        public DbSet<Documento> Documentos { get; set; }
        public DbSet<NodoCampus> NodosCampus { get; set; }
        public DbSet<RutaCampus> RutasCampus { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Nombres exactos de las tablas en SQL Server
            modelBuilder.Entity<Rol>().ToTable("Roles");
            modelBuilder.Entity<Usuario>().ToTable("Usuarios");
            modelBuilder.Entity<Estudiante>().ToTable("Estudiantes");
            modelBuilder.Entity<PersonalAdministrativo>().ToTable("Personal");
            modelBuilder.Entity<Tramite>().ToTable("Tramites");
            modelBuilder.Entity<HistorialTramite>().ToTable("HistorialTramite");
            modelBuilder.Entity<Ventanilla>().ToTable("Ventanillas");
            modelBuilder.Entity<Turno>().ToTable("Turnos");
            modelBuilder.Entity<AccionUsuario>().ToTable("AccionesUsuario");
            modelBuilder.Entity<CategoriaDoc>().ToTable("CategoriasDoc");
            modelBuilder.Entity<Documento>().ToTable("Documentos");
            modelBuilder.Entity<NodoCampus>().ToTable("NodosCampus");
            modelBuilder.Entity<RutaCampus>().ToTable("RutasCampus");

            // Usuario → Estudiante (uno a uno)
            modelBuilder.Entity<Estudiante>()
                .HasOne(e => e.Usuario)
                .WithOne(u => u.Estudiante)
                .HasForeignKey<Estudiante>(e => e.IdUsuario)
                .OnDelete(DeleteBehavior.Restrict);

            // Usuario → PersonalAdministrativo (uno a uno)
            modelBuilder.Entity<PersonalAdministrativo>()
                .HasOne(p => p.Usuario)
                .WithOne(u => u.Personal)
                .HasForeignKey<PersonalAdministrativo>(p => p.IdUsuario)
                .OnDelete(DeleteBehavior.Restrict);

            // NodoCampus → RutasCampus (origen)
            modelBuilder.Entity<RutaCampus>()
                .HasOne(r => r.NodoOrigen)
                .WithMany(n => n.RutasOrigen)
                .HasForeignKey(r => r.IdNodoOrigen)
                .OnDelete(DeleteBehavior.Restrict);

            // NodoCampus → RutasCampus (destino)
            modelBuilder.Entity<RutaCampus>()
                .HasOne(r => r.NodoDestino)
                .WithMany(n => n.RutasDestino)
                .HasForeignKey(r => r.IdNodoDestino)
                .OnDelete(DeleteBehavior.Restrict);

            // CategoriaDoc → CategoriaDoc (autorreferencia árbol)
            modelBuilder.Entity<CategoriaDoc>()
                .HasOne(c => c.CategoriaPadre)
                .WithMany(c => c.SubCategorias)
                .HasForeignKey(c => c.IdCategoriaPadre)
                .OnDelete(DeleteBehavior.Restrict);

            // Usuario → AccionesUsuario (uno a muchos)
            modelBuilder.Entity<AccionUsuario>()
                .HasOne(a => a.Usuario)
                .WithMany()
                .HasForeignKey(a => a.IdUsuario)
                .OnDelete(DeleteBehavior.Restrict);

            // Usuario → HistorialTramite (uno a muchos)
            modelBuilder.Entity<HistorialTramite>()
                .HasOne(h => h.Usuario)
                .WithMany()
                .HasForeignKey(h => h.IdUsuario)
                .OnDelete(DeleteBehavior.Restrict);

            // Usuario → Documentos (uno a muchos)
            modelBuilder.Entity<Documento>()
                .HasOne(d => d.Usuario)
                .WithMany()
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}