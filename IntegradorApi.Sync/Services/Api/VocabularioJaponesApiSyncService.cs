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

public class VocabularioJaponesApiSyncService : SyncApiServiceBase<VocabularioJapones> {
    private readonly ILogger _logger;
    private TextoJaponesApiService? _api;

    public VocabularioJaponesApiSyncService(Connection connection, ILogger logger) : base(connection) {
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

    public override async Task GetAsync(DateTime since, ProgressCallback<VocabularioJapones> onPageReceived) {
        _logger.Information("Iniciando busca de atualizações de VocabularioJapones para a conexão {Description}", Connection.Description);

        int currentPage = 0;
        bool hasNextPage;

        do {
            var pagedResponse = await _api!.GetUpdatesVocabularioAsync(since, currentPage);

            if (pagedResponse?.Content == null || !pagedResponse.Content.Any()) {
                hasNextPage = false;
            } else {
                var entities = Mapper.Map<List<VocabularioJapones>>(pagedResponse.Content);
                await onPageReceived.Invoke(entities, "vocabulario");
                hasNextPage = !string.IsNullOrEmpty(pagedResponse.Links?.Next?.Href);
                currentPage++;
            }
        } while (hasNextPage);
    }

    public override async Task SaveAsync(List<VocabularioJapones> entities, string extra) {
        var dtos = Mapper.Map<List<VocabularioDto>>(entities);
        await _api!.SendAsync(dtos);
    }

    public override async Task DeleteAsync(List<VocabularioJapones> entities, string extra) {
        var dtos = Mapper.Map<List<VocabularioDto>>(entities);
        await _api!.DeleteAsync(dtos);
    }
}
