using Microsoft.Extensions.Configuration;
using Serilog;
using System;

namespace IntegradorApi;

public static class AppConfig {
    public static IConfiguration Configuration { get; }

    static AppConfig() {
        Log.Information("Realizando a leitura das configurações.");
        Configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();
    }
}
