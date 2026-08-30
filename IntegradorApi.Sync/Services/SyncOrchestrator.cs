using IntegradorApi.Data.Enums;
using IntegradorApi.Data.Models;
using IntegradorApi.Data.Services;
using IntegradorApi.Sync.Interfaces;
using IntegradorApi.Sync.Services.Api;
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

    private List<ISyncRunner> GetServices(Connection connection) {
        switch (connection.TypeConnection) {
            case ConnectionType.MYSQL:
                return connection.TypeIntegration switch {
                    IntegrationType.MANGA_EXTRACTOR => new List<ISyncRunner> { new MangaDataSyncService(connection, _logger) },
                    IntegrationType.NOVEL_EXTRACTOR => new List<ISyncRunner> { new NovelDataSyncService(connection, _logger) },
                    IntegrationType.DECKSUBTITLE => new List<ISyncRunner> { new DeckSubtitleDataSyncService(connection, _logger) },
                    IntegrationType.COMICINFO => new List<ISyncRunner> { new ComicInfoDataSyncService(connection, _logger) },
                    _ => new List<ISyncRunner>(),
                };
            case ConnectionType.APIREST:
                return connection.TypeIntegration switch {
                    IntegrationType.MANGA_EXTRACTOR => new List<ISyncRunner> { new MangaApiSyncService(connection, _logger) },
                    IntegrationType.NOVEL_EXTRACTOR => new List<ISyncRunner> { new NovelApiSyncService(connection, _logger) },
                    IntegrationType.DECKSUBTITLE => new List<ISyncRunner> { new DeckSubtitleApiSyncService(connection, _logger) },
                    IntegrationType.COMICINFO => new List<ISyncRunner> { new ComicInfoApiSyncService(connection, _logger) },
                    IntegrationType.TEXTO_JAPONES => new List<ISyncRunner> {
                        new VocabularioJaponesApiSyncService(connection, _logger),
                        new RevisarJaponesApiSyncService(connection, _logger),
                        new KanjiInfoApiSyncService(connection, _logger),
                        new KanjaxPtApiSyncService(connection, _logger),
                        new ExclusaoJaponesApiSyncService(connection, _logger),
                        new EstatisticaJaponesApiSyncService(connection, _logger)
                    },
                    IntegrationType.TEXTO_INGLES => new List<ISyncRunner> {
                        new VocabularioInglesApiSyncService(connection, _logger),
                        new RevisarInglesApiSyncService(connection, _logger),
                        new ExclusaoInglesApiSyncService(connection, _logger),
                        new ValidoInglesApiSyncService(connection, _logger)
                    },
                    _ => new List<ISyncRunner>(),
                };
            default:
                throw new InvalidOperationException("Conexão com banco ainda não implementada.");
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

            var originServices = GetServices(connectionOrigin);
            if (!originServices.Any()) continue;

            var destinationServicesList = destinationsForOrigin.SelectMany(dest => GetServices(dest)).ToList();

            foreach (var originService in originServices) {
                try {
                    await originService.RunSyncAsync(destinationServicesList, connectionOrigin, _databaseService, _logger);
                } catch (Exception ex) {
                    _logger.Error(ex, "Erro ao sincronizar recurso {ResourceName} para a conexão {Description}", originService.ResourceName, connectionOrigin.Description);
                }
            }
        }
        _logger.Information("Orquestrador de sincronização finalizado.");
    }
}