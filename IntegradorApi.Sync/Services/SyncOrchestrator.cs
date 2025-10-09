using IntegradorApi.Data.Enums;
using IntegradorApi.Data.Models;
using IntegradorApi.Data.Services;
using IntegradorApi.Sync.Interfaces;
using IntegradorApi.Sync.Services.Data;
using Serilog;

namespace IntegradorApi.Sync.Services;

public class SyncOrchestrator {
    private readonly DatabaseService _databaseService;
    private readonly ILogger _logger;

    public SyncOrchestrator(DatabaseService databaseService, ILogger logger) {
        _databaseService = databaseService;
        _logger = logger;
    }

    private SyncServiceBase<T>? GetService<T>(Connection connection) where T : Entity {
        switch (connection.TypeConnection) {
            case ConnectionType.MySql:
                return connection.TypeIntegration switch {
                    IntegrationType.MangaExtractor => new MangaDataSyncService(connection, _logger) as SyncServiceBase<T>,
                    IntegrationType.NovelExtractor => null,
                    _ => throw new InvalidOperationException("Integração ainda não impementada."),
                };
            case ConnectionType.RestApi:
                return connection.TypeIntegration switch {
                    IntegrationType.MangaExtractor => new MangaApiSyncService(connection, _logger) as SyncServiceBase<T>,
                    IntegrationType.NovelExtractor => null,
                    _ => throw new InvalidOperationException("Integração ainda não impementada."),
                };
            default:
                throw new InvalidOperationException("Conexão com banco ainda não impementado.");
        }
    }

    /// <summary>
    /// Executa a sincronização para todas as conexões de origem ativas.
    /// </summary>
    public async Task RunAllActiveSyncsAsync() {
        _logger.Information("Iniciando orquestrador de sincronização...");
        var connectionsOrigin = await _databaseService.GetConnectionsAsync();
        var connectionsDestination = await _databaseService.GetConnectionsAsync();

        foreach (var connectionOrigin in connectionsOrigin.Where(c => c.Enabled)) {
            var connectionDestination = connectionsDestination.Find(c => c.Enabled && c.TypeIntegration == connectionOrigin.TypeIntegration);
            if (connectionDestination == null)
                continue;

            try {
                var origin = GetService<Entity>(connectionOrigin);
                var destination = GetService<Entity>(connectionDestination);

                if (origin == null || destination == null)
                    continue;

                await RunSyncForConnectionAsync(origin, destination, connectionOrigin);
            } catch (Exception ex) {
                _logger.Error(ex, "Falha crítica ao sincronizar a conexão {Description}", connectionOrigin.Description);
            }
        }
        _logger.Information("Orquestrador de sincronização finalizado.");
    }

    /// <summary>
    /// O método genérico que executa o fluxo de sincronização para uma conexão.
    /// </summary>
    private async Task RunSyncForConnectionAsync<T>(ISyncService<T> serviceOrigin, ISyncService<T> serviceDestination, Connection connection) where T : Entity {
        _logger.Information("Processando conexão: {Description}", connection.Description);

        DateTime lastSyncDate = await _databaseService.GetLastSyncDateAsync(connection.Id);
        DateTime sinc = DateTime.UtcNow;

        async Task HandlePage(List<T> pageToSave, String extra) {
            await serviceDestination.SaveAsync(pageToSave, extra);

            if (connection.Delete)
                await serviceOrigin.DeleteAsync(pageToSave, extra);
        }

        await serviceOrigin.GetAsync(lastSyncDate, HandlePage);

        await _databaseService.UpdateLastSyncDateAsync(connection.Id, sinc);

        _logger.Information("--------------------------------------------------------------------------------");
    }
}