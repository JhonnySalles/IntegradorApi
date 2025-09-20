using Serilog;
using Serilog.Core;
using System;
using System.IO;

namespace IntegradorApi.Services;

public static class LogManager {
  public static Logger Logger { get; }

  static LogManager() {
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
        .WriteTo.Sink(new UISink())
        .CreateLogger();

    Log.Logger = Logger;
  }
}
