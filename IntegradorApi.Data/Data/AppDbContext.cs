using IntegradorApi.Models;
using Microsoft.EntityFrameworkCore;

namespace IntegradorApi.Data;

public class AppDbContext : DbContext {
    public DbSet<Connection> Connections { get; set; }
    public DbSet<Sincronization> Sincronizations { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Connection>().HasKey(s => s.Id);
        modelBuilder.Entity<Sincronization>().HasKey(s => s.Id);
    }

}
