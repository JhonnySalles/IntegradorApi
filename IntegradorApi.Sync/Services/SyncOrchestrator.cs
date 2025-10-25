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
            case ConnectionType.MYSQL:
                return connection.TypeIntegration switch {
                    IntegrationType.MANGA_EXTRACTOR => new MangaDataSyncService(connection, _logger) as SyncServiceBase<T>,
                    IntegrationType.NOVEL_EXTRACTOR => new NovelDataSyncService(connection, _logger) as SyncServiceBase<T>,
                    IntegrationType.DECKSUBTITLE => new DeckSubtitleDataSyncService(connection, _logger) as SyncServiceBase<T>,
                    IntegrationType.COMICINFO => new ComicInfoDataSyncService(connection, _logger) as SyncServiceBase<T>,
                    _ => throw new InvalidOperationException("Integração ainda não impementada."),
                };
            case ConnectionType.APIREST:
                return connection.TypeIntegration switch {
                    IntegrationType.MANGA_EXTRACTOR => new MangaApiSyncService(connection, _logger) as SyncServiceBase<T>,
                    IntegrationType.NOVEL_EXTRACTOR => new NovelApiSyncService(connection, _logger) as SyncServiceBase<T>,
                    IntegrationType.DECKSUBTITLE => new DeckSubtitleApiSyncService(connection, _logger) as SyncServiceBase<T>,
                    IntegrationType.COMICINFO => new ComicInfoApiSyncService(connection, _logger) as SyncServiceBase<T>,
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
        var connectionsOrigin = await _databaseService.GetOriginConnectionsAsync();
        var connectionsDestination = await _databaseService.GetDestinationConnectionsAsync();

        foreach (var connectionOrigin in connectionsOrigin.Where(c => c.Enabled)) {
            var destinationsForOrigin = connectionsDestination
                .Where(dest => dest.Enabled && dest.TypeIntegration == connectionOrigin.TypeIntegration)
                .ToList();

            if (!destinationsForOrigin.Any()) {
                _logger.Warning("Nenhuma conexão de destino ativa encontrada para a origem {Description}", connectionOrigin.Description);
                continue;
            }

            try {
                var originService = GetService<Entity>(connectionOrigin);
                var destinationServices = destinationsForOrigin.Select(dest => GetService<Entity>(dest))
                                            .Where(service => service != null)
                                            .Cast<ISyncService<Entity>>()
                                            .ToList();

                if (originService == null || !destinationServices.Any())
                    continue;

                await RunSyncForConnectionAsync(originService, destinationServices, connectionOrigin);
            } catch (Exception ex) {
                _logger.Error(ex, "Falha crítica ao sincronizar a conexão {Description}", connectionOrigin.Description);
            }
        }
        _logger.Information("Orquestrador de sincronização finalizado.");
    }

    /// <summary>
    /// O método genérico que executa o fluxo de sincronização para uma conexão.
    /// </summary>
    private async Task RunSyncForConnectionAsync<T>(ISyncService<T> serviceOrigin, List<ISyncService<T>> serviceDestinations, Connection connection) where T : Entity {
        _logger.Information("Processando conexão: {Description}", connection.Description);

        DateTime lastSyncDate = await _databaseService.GetLastSyncDateAsync(connection.Id);
        DateTime sinc = DateTime.UtcNow;

        async Task HandlePage(List<T> pageToSave, String extra) {
            foreach (var destinationService in serviceDestinations) {
                _logger.Information("Enviando registros para {Description}", destinationService.Description);
                await destinationService.SaveAsync(pageToSave, extra);
            }

            if (connection.Delete) {
                _logger.Information("Deletando registros de {Description}", serviceOrigin.Description);
                await serviceOrigin.DeleteAsync(pageToSave, extra);
            }
        }

        await serviceOrigin.GetAsync(lastSyncDate, HandlePage);

        await _databaseService.UpdateLastSyncDateAsync(connection.Id, sinc);

        _logger.Information("--------------------------------------------------------------------------------");
    }
}