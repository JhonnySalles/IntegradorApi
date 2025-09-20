using IntegradorApi.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace IntegradorApi.Data;

public class AppDbContext : DbContext {
    public DbSet<Connections> DbSetConnections { get; set; }
    public DbSet<Sincronization> DbSetSincronizations { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {
    }

    public AppDbContext() {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
        if (!optionsBuilder.IsConfigured) {
            var dbConfig = AppConfig.Configuration.GetSection("ConnectionStrings:LocalDatabase");

            string connectionString = $"Server={dbConfig["Address"]};" +
                                      $"Port={dbConfig["Port"]};" +
                                      $"Database=integradorapi;" +
                                      $"Uid={dbConfig["User"]};" +
                                      $"Pwd={dbConfig["Password"]};";

            var serverVersion = new MySqlServerVersion(new Version(8, 1, 0));

            optionsBuilder.UseMySql(connectionString, serverVersion);
        }
    }
}
