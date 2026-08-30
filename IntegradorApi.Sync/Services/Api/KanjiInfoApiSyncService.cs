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

public class KanjiInfoApiSyncService : SyncApiServiceBase<KanjiInfo> {
    private readonly ILogger _logger;
    private TextoJaponesApiService? _api;

    public KanjiInfoApiSyncService(Connection connection, ILogger logger) : base(connection) {
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

    public override async Task GetAsync(DateTime since, ProgressCallback<KanjiInfo> onPageReceived) {
        _logger.Information("Iniciando busca de atualizações de KanjiInfo para a conexão {Description}", Connection.Description);

        int currentPage = 0;
        bool hasNextPage;

        do {
            var pagedResponse = await _api!.GetUpdatesKanjiInfoAsync(since, currentPage);

            if (pagedResponse?.Content == null || !pagedResponse.Content.Any()) {
                hasNextPage = false;
            } else {
                var entities = Mapper.Map<List<KanjiInfo>>(pagedResponse.Content);
                await onPageReceived.Invoke(entities, "words_kanji_info");
                hasNextPage = !string.IsNullOrEmpty(pagedResponse.Links?.Next?.Href);
                currentPage++;
            }
        } while (hasNextPage);
    }

    public override async Task SaveAsync(List<KanjiInfo> entities, string extra) {
        var dtos = Mapper.Map<List<KanjiInfoDto>>(entities);
        await _api!.SendAsync(dtos);
    }

    public override async Task DeleteAsync(List<KanjiInfo> entities, string extra) {
        var dtos = Mapper.Map<List<KanjiInfoDto>>(entities);
        await _api!.DeleteAsync(dtos);
    }
}
