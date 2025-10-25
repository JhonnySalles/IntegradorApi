using AutoMapper;
using IntegradorApi.Api.Models;
using IntegradorApi.Api.Services;
using IntegradorApi.Data.Models;
using IntegradorApi.Data.Models.NovelExtractor;
using IntegradorApi.Data.Services;
using IntegradorApi.Sync.Interfaces;
using Microsoft.Extensions.Logging.Abstractions;
using Serilog;

namespace IntegradorApi.Sync.Services.Data;

public class NovelApiSyncService : SyncApiServiceBase<NovelVolume> {
    private readonly ILogger _logger;
    private NovelApiService _api;

    public NovelApiSyncService(Connection connection, ILogger logger) : base(connection) {
        _logger = logger;
    }
    protected override async void Initialize() {
        var apiClient = new ApiClientService(Connection, _logger);
        _api = new NovelApiService(apiClient, _logger);
    }
    protected override MapperConfiguration CreateMapperInstance() {
        return new MapperConfiguration(cfg => {
            cfg.AddProfile(new NovelMappingProfile());
        }, NullLoggerFactory.Instance);
    }

    public override async Task GetAsync(DateTime since, ProgressCallback<NovelVolume> onPageReceived) {
        _logger.Information("Iniciando 'Loading' de Novels para a conexão {Description}", Connection.Description);

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
                    var entities = Mapper.Map<List<NovelVolume>>(pagedResponse.Content);
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

    public override async Task SaveAsync(List<NovelVolume> entities, String extra) {
        _logger.Information("Iniciando 'Save' de {Count} volumes de Novels", entities.Count);
        var dtos = Mapper.Map<List<NovelVolumeDto>>(entities);
        await _api.SendVolumesAsync(extra, dtos);
    }

    public override async Task DeleteAsync(List<NovelVolume> entities, String extra) {
        _logger.Information("Iniciando 'Delete' de {Count} volumes de Novels", entities.Count);
        var dtos = Mapper.Map<List<NovelVolumeDto>>(entities);
        await _api.DeleteVolumesAsync(extra, dtos);
    }
}