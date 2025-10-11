using IntegradorApi.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace IntegradorApi.Data;

public class AppDbContext : DbContext {
    public DbSet<Connection> Connections { get; set; }
    public DbSet<Sincronization> Sincronizations { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Connection>(entity => {
            entity.HasKey(c => c.Id);
            entity.Property(c => c.TypeIntegration).HasConversion<string>();
            entity.Property(c => c.TypeDataSource).HasConversion<string>();
            entity.Property(c => c.TypeConnection).HasConversion<string>();
        });
        modelBuilder.Entity<Sincronization>().HasKey(s => s.Id);
    }

}
