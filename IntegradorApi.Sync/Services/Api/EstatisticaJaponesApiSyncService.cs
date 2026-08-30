using AutoMapper;
using IntegradorApi.Api.Models;
using IntegradorApi.Api.Services;
using IntegradorApi.Data.Models;
using IntegradorApi.Data.Models.TextoJapones;
using IntegradorApi.Data.Services;
using IntegradorApi.Sync.Interfaces;
using IntegradorApi.Sync.Mappings;
using Microsoft.Extensions.Logging.Abstractions;
using Serilog;

namespace IntegradorApi.Sync.Services.Api;

public class EstatisticaJaponesApiSyncService : SyncApiServiceBase<EstatisticaJapones> {
    private readonly ILogger _logger;
    private TextoJaponesApiService? _api;

    public EstatisticaJaponesApiSyncService(Connection connection, ILogger logger) : base(connection) {
        _logger = logger;
    }

    protected override void Initialize() {
        var apiClient = new ApiClientService(Connection, _logger);
        _api = new TextoJaponesApiService(apiClient, _logger);
    }

    protected override MapperConfiguration CreateMapperInstance() {
        return new MapperConfiguration(cfg => {
            cfg.AddProfile(new TextoJaponesMappingProfile());
        }, NullLoggerFactory.Instance);
    }

    public override async Task GetAsync(DateTime since, ProgressCallback<EstatisticaJapones> onPageReceived) {
        _logger.Information("Iniciando busca de atualizações de EstatisticaJapones para a conexão {Description}", Connection.Description);

        int currentPage = 0;
        bool hasNextPage;

        do {
            var pagedResponse = await _api!.GetUpdatesEstatisticaAsync(since, currentPage);

            if (pagedResponse?.Content == null || !pagedResponse.Content.Any()) {
                hasNextPage = false;
            } else {
                var entities = Mapper.Map<List<EstatisticaJapones>>(pagedResponse.Content);
                await onPageReceived.Invoke(entities, "estatistica");
                hasNextPage = !string.IsNullOrEmpty(pagedResponse.Links?.Next?.Href);
                currentPage++;
            }
        } while (hasNextPage);
    }

    public override async Task SaveAsync(List<EstatisticaJapones> entities, string extra) {
        var dtos = Mapper.Map<List<EstatisticaDto>>(entities);
        await _api!.SendAsync(dtos);
    }

    public override async Task DeleteAsync(List<EstatisticaJapones> entities, string extra) {
        var dtos = Mapper.Map<List<EstatisticaDto>>(entities);
        await _api!.DeleteAsync(dtos);
    }
}
