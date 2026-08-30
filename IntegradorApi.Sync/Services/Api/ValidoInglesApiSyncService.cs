using AutoMapper;
using IntegradorApi.Api.Models;
using IntegradorApi.Api.Services;
using IntegradorApi.Data.Models;
using IntegradorApi.Data.Models.TextoIngles;
using IntegradorApi.Data.Services;
using IntegradorApi.Sync.Interfaces;
using IntegradorApi.Sync.Mappings;
using Microsoft.Extensions.Logging.Abstractions;
using Serilog;
using ValidoDto = IntegradorApi.Api.Models.TextoIngles.ValidoDto;

namespace IntegradorApi.Sync.Services.Api;

public class ValidoInglesApiSyncService : SyncApiServiceBase<ValidoIngles> {
    private readonly ILogger _logger;
    private TextoInglesApiService? _api;

    public ValidoInglesApiSyncService(Connection connection, ILogger logger) : base(connection) {
        _logger = logger;
    }

    protected override void Initialize() {
        var apiClient = new ApiClientService(Connection, _logger);
        _api = new TextoInglesApiService(apiClient, _logger);
    }

    protected override MapperConfiguration CreateMapperInstance() {
        return new MapperConfiguration(cfg => {
            cfg.AddProfile(new TextoInglesMappingProfile());
        }, NullLoggerFactory.Instance);
    }

    public override async Task GetAsync(DateTime since, ProgressCallback<ValidoIngles> onPageReceived) {
        _logger.Information("Iniciando busca de atualizações de ValidoIngles para a conexão {Description}", Connection.Description);

        int currentPage = 0;
        bool hasNextPage;

        do {
            var pagedResponse = await _api!.GetUpdatesValidoAsync(since, currentPage);

            if (pagedResponse?.Content == null || !pagedResponse.Content.Any()) {
                hasNextPage = false;
            } else {
                var entities = Mapper.Map<List<ValidoIngles>>(pagedResponse.Content);
                await onPageReceived.Invoke(entities, "valido");
                hasNextPage = !string.IsNullOrEmpty(pagedResponse.Links?.Next?.Href);
                currentPage++;
            }
        } while (hasNextPage);
    }

    public override async Task SaveAsync(List<ValidoIngles> entities, string extra) {
        var dtos = Mapper.Map<List<ValidoDto>>(entities);
        await _api!.SendAsync(dtos);
    }

    public override async Task DeleteAsync(List<ValidoIngles> entities, string extra) {
        var dtos = Mapper.Map<List<ValidoDto>>(entities);
        await _api!.DeleteAsync(dtos);
    }
}
