using AutoMapper;
using IntegradorApi.Api.Models;
using IntegradorApi.Api.Services;
using IntegradorApi.Data.Models;
using IntegradorApi.Data.Models.ProcessaTexto;
using IntegradorApi.Data.Services;
using IntegradorApi.Sync.Interfaces;
using Microsoft.Extensions.Logging.Abstractions;
using Serilog;

namespace IntegradorApi.Sync.Services.Data;

public class ComicInfoApiSyncService : SyncApiServiceBase<ComicInfo> {
    private readonly ILogger _logger;
    private ComicInfoApiService? _api;

    public ComicInfoApiSyncService(Connection connection, ILogger logger) : base(connection) {
        _logger = logger;
    }
    protected override void Initialize() {
        var apiClient = new ApiClientService(Connection, _logger);
        _api = new ComicInfoApiService(apiClient, _logger);
    }

    protected override MapperConfiguration CreateMapperInstance() {
        return new MapperConfiguration(cfg => {
            cfg.AddProfile(new ComicInfoMappingProfile());
        }, NullLoggerFactory.Instance);
    }

    public override async Task GetAsync(DateTime since, ProgressCallback<ComicInfo> onPageReceived) {
        _logger.Information("Iniciando 'Loading' de Comic Info para a conexão {Description}", Connection.Description);

        int currentPage = 0;
        bool hasNextPage;

        do {
            var pagedResponse = await _api!.GetUpdatesAsync(since, currentPage);

            if (pagedResponse?.Content == null || !pagedResponse.Content.Any()) {
                _logger.Information("Nenhum dado novo encontrado na página {Page}", currentPage);
                hasNextPage = false;
            } else {
                var entities = Mapper.Map<List<ComicInfo>>(pagedResponse.Content);
                var pageInfo = pagedResponse.Page;
                float progressPercentage = pageInfo != null && pageInfo.TotalPages > 0 ? (float)(pageInfo.Number + 1) / pageInfo.TotalPages : 1.0f;
                string progressText = pageInfo != null ? $"Página {pageInfo.Number + 1} de {pageInfo.TotalPages} ({pagedResponse.Content.Count} registros)" : "Página única";

                _logger.Information(progressText);

                await onPageReceived.Invoke(entities, "");
                hasNextPage = !string.IsNullOrEmpty(pagedResponse.Links?.Next?.Href);
                currentPage++;
            }

        } while (hasNextPage);
    }

    public override async Task SaveAsync(List<ComicInfo> entities, String extra) {
        _logger.Information("Iniciando 'Save' de {Count} registros de Comic Info", entities.Count);
        var dtos = Mapper.Map<List<ComicInfoDto>>(entities);
        await _api!.SendAsync(dtos);
    }

    public override async Task DeleteAsync(List<ComicInfo> entities, String extra) {
        _logger.Information("Iniciando 'Delete' de {Count} registros de Comic Info", entities.Count);
        var dtos = Mapper.Map<List<ComicInfoDto>>(entities);
        await _api!.DeleteAsync(dtos);
    }
}