using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace IntegradorApi.Data;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppDbContext> {
    public AppDbContext CreateDbContext(string[] args) {
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        var dbConfig = configuration.GetSection("ConnectionStrings:LocalDatabase");
        string connectionString = $"Server={dbConfig["Address"]};" +
                                  $"Port={dbConfig["Port"]};" +
                                  $"Database=integradorapi;" +
                                  $"Uid={dbConfig["User"]};" +
                                  $"Pwd={dbConfig["Password"]};";

        var builder = new DbContextOptionsBuilder<AppDbContext>();
        var serverVersion = new MySqlServerVersion(new Version(8, 1, 0));
        builder.UseMySql(connectionString, serverVersion);

        return new AppDbContext(builder.Options);
    }
}