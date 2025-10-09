using IntegradorApi.Api.Services;
using IntegradorApi.Data.Enums;
using IntegradorApi.Data.Models;
using IntegradorApi.Data.Models.MangaExtractor;
using IntegradorApi.Data.Services;
using IntegradorApi.Sync.Interfaces;
using Serilog;

namespace IntegradorApi.Sync.Services.Data;

public class MangaApiSyncService : SyncServiceBase<MangaVolume> {
    private readonly ILogger _logger;
    private MangaApiService _api;

    public MangaApiSyncService(Connection connection, ILogger logger) : base(connection) {
        _logger = logger;
    }
    protected override async void initialize() {
        var apiClient = new ApiClientService(Connection, _logger);
        _api = new MangaApiService(apiClient, _logger);
    }

    public override async Task GetAsync(DateTime since, ProgressCallback<MangaVolume> onPageReceived) {
        _logger.Information("Iniciando 'Loading' de Mangás para a conexão {Description}", Connection.Description);

        var tables = await _api.GetTablesAsync();
        if (tables == null || !tables.Any()) {
            _logger.Warning("Nenhuma tabela encontrada para a conexão {Description}", Connection.Description);
            return;
        }

        foreach (var table in tables) {
            _logger.Information("Processando tabela: {TableName}", table);
            int currentPage = 0;
            bool hasNextPage;

            do {
                var pagedResponse = await _api.GetUpdatesAsync(table, since, currentPage);

                if (pagedResponse?.Content == null || !pagedResponse.Content.Any()) {
                    _logger.Information("Nenhum dado novo encontrado na página {Page} para a tabela {Table}", currentPage, table);
                    hasNextPage = false;
                } else {
                    var entities = pagedResponse.Content.Select(dto => new MangaVolume {
                        Id = dto.Id,
                        Manga = dto.Manga,
                        Volume = dto.Volume,
                        Lingua = Enum.Parse<Linguagens>(dto.Lingua, true),
                        Arquivo = dto.Arquivo,
                        Processado = dto.Processado
                    }).ToList();

                    var pageInfo = pagedResponse.Page;
                    float progressPercentage = pageInfo != null && pageInfo.TotalPages > 0 ? (float)(pageInfo.Number + 1) / pageInfo.TotalPages : 1.0f;
                    string progressText = pageInfo != null ? $"Página {pageInfo.Number + 1} de {pageInfo.TotalPages} ({pagedResponse.Content.Count} registros)" : "Página única";

                    _logger.Information(progressText);

                    await onPageReceived.Invoke(entities, table);
                    hasNextPage = !string.IsNullOrEmpty(pagedResponse.Links?.Next?.Href);
                    currentPage++;
                }

            } while (hasNextPage);

            _logger.Information("Processamento da tabela {Table} concluído.", table);
        }
    }

    public override async Task SaveAsync(List<MangaVolume> entities, String extra) {
        _logger.Information("Iniciando 'Save' de {Count} volumes de Mangá", entities.Count);
        await _api.SendVolumesAsync(extra, entities);
    }

    public override async Task DeleteAsync(List<MangaVolume> entities, String extra) {
        _logger.Information("Iniciando 'Delete' de {Count} volumes de Mangá", entities.Count);
        await _api.DeleteVolumesAsync(extra, entities);
    }
}