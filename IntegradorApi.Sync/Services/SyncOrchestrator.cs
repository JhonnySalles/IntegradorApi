using IntegradorApi.Data.Enums;
using IntegradorApi.Data.Models;
using IntegradorApi.Data.Services;
using IntegradorApi.Sync.Interfaces;
using IntegradorApi.Sync.Services.Data;
using Serilog;

namespace IntegradorApi.Sync.Services;

public class SyncOrchestrator {
    private readonly DatabaseService _databaseService; // Para buscar conexões e datas
    private readonly ILogger _logger;

    public SyncOrchestrator(DatabaseService databaseService, ILogger logger) {
        _databaseService = databaseService;
        _logger = logger;
    }

    /// <summary>
    /// Executa a sincronização para todas as conexões de origem ativas.
    /// </summary>
    public async Task RunAllActiveSyncsAsync() {
        _logger.Information("Iniciando orquestrador de sincronização...");
        var activeConnections = await _databaseService.GetConnectionsAsync(); // Assumindo que este método retorna todas

        foreach (var connection in activeConnections.Where(c => c.Enabled)) {
            try {
                // Lógica de fábrica para decidir qual serviço usar
                switch (connection.TypeConnection) {
                    case ConnectionType.MySql:
                        var mangaService = new MangaDataSyncService(connection, _logger);
                        await RunSyncForConnectionAsync(mangaService, connection);
                        break;

                    case ConnectionType.RestApi: // Supondo que Novel use RestApi
                        // var novelService = new NovelSyncService(connection, _logger);
                        // await RunSyncForConnectionAsync(novelService, connection);
                        _logger.Warning("Sincronização para Novel (RestApi) ainda não implementada.");
                        break;
                }
            } catch (Exception ex) {
                _logger.Error(ex, "Falha crítica ao sincronizar a conexão {Description}", connection.Description);
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

        async Task HandlePage(List<T> pageToSave) {
            await serviceDestination.SaveAsync(pageToSave);

            if (true)
                serviceOrigin.DeleteAsync(pageToSave);
        }

        await serviceOrigin.GetAsync(lastSyncDate, HandlePage);

        await _databaseService.UpdateLastSyncDateAsync(connection.Id, sinc);

        _logger.Information("--------------------------------------------------------------------------------");
    }
}