using Serilog;
using Serilog.Core;
using System;
using System.IO;

namespace IntegradorApi.Services;

public static class LogManager {
    public static Logger Logger { get; }

    static LogManager() {
        // Define o caminho para a pasta de logs dentro do diretório do AppData do usuário
        // Isso evita problemas de permissão de escrita em pastas como C:\Program Files
        string logPath = Path.Combine(AppContext.BaseDirectory, "log", "log-.txt");

        Directory.CreateDirectory(Path.GetDirectoryName(logPath));

        Logger = new LoggerConfiguration()
            .MinimumLevel.Verbose()
            .WriteTo.File(
                path: logPath,
                rollingInterval: RollingInterval.Day,
                rollOnFileSizeLimit: true,
                fileSizeLimitBytes: 10 * 1024 * 1024,
                retainedFileCountLimit: 31,
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}"
            )
            .WriteTo.Console()
            .WriteTo.Console()
            .CreateLogger();

        Log.Logger = Logger;
    }
}
