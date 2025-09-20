using IntegradorApi.Models;
using Microsoft.EntityFrameworkCore;

namespace IntegradorApi.Data;

public class AppDbContext : DbContext {
    public DbSet<Connection> DbSetConnections { get; set; }
    public DbSet<Sincronization> DbSetSincronizations { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {
    }

    public AppDbContext() {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Connection>().HasKey(c => c.Id);
        modelBuilder.Entity<Sincronization>().HasKey(s => s.Id);
    }

}
