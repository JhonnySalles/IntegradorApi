using IntegradorApi.Data.Models.TextoIngles;
using Microsoft.EntityFrameworkCore;

namespace IntegradorApi.Data.Data;

public class TextoInglesDbContext : DbContext {
    public DbSet<VocabularioIngles> Vocabularios { get; set; }
    public DbSet<ValidoIngles> Validos { get; set; }
    public DbSet<RevisarIngles> Revisar { get; set; }
    public DbSet<ExclusaoIngles> Exclusoes { get; set; }

    public TextoInglesDbContext(DbContextOptions<TextoInglesDbContext> options) : base(options) {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        base.OnModelCreating(modelBuilder);
    }
}
