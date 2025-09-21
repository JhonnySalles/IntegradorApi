using IntegradorApi.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
//using System.Diagnostics; //-- Forçar o debug

namespace IntegradorApi.Data;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppDbContext> {
    public AppDbContext CreateDbContext(string[] args) {
        //Debugger.Launch(); //-- Forçar o debug
        var startupProjectName = "IntegradorApi";

        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile($"{startupProjectName}/../appsettings.json", optional: false)
            .Build();

        var dbConfig = configuration.GetSection("ConnectionStrings:LocalDatabase");
        string connectionString = $"Server={dbConfig["Address"]};" +
                                  $"Port={dbConfig["Port"]};" +
                                  $"Database={GlobalConstants.DatabaseName};" +
                                  $"Uid={dbConfig["User"]};" +
                                  $"Pwd={dbConfig["Password"]};";

        var builder = new DbContextOptionsBuilder<AppDbContext>();
        builder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));

        return new AppDbContext(builder.Options);
    }
}
