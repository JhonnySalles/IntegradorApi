using IntegradorApi.Data.Models;
using IntegradorApi.Data.Models.ProcessaTexto;
using IntegradorApi.Data.Services;
using IntegradorApi.Sync.Interfaces;
using MySqlConnector;
using Serilog;

namespace IntegradorApi.Sync.Services.Data;

public class ComicInfoDataSyncService : SyncDataServiceBase<ComicInfo> {
    private readonly ILogger _logger;
    private ComicInfoDataService? _service;

    public ComicInfoDataSyncService(Connection connection, ILogger logger) : base(connection) {
        _logger = logger;
    }

    protected override async void Initialize() {
        var dbConnection = new MySqlConnection(Connection.Address);
        await dbConnection.OpenAsync();
        _service = new ComicInfoDataService(dbConnection, _logger);
    }

    public override async Task GetAsync(DateTime since, ProgressCallback<ComicInfo> onPageReceived) {
        _logger.Information("Iniciando consulta de Comic Info para a conexão {Description}", Connection.Description);

        var list = await _service!.GetAsync(since);
        if (list == null || !list.Any()) {
            _logger.Warning("Nenhum dado encontrada para a conexão {Description}", Connection.Description);
            return;
        }

        await onPageReceived.Invoke(list, "");
    }

    public override async Task SaveAsync(List<ComicInfo> entities, String extra) {
        await _service!.SaveAsync(entities, extra);
    }

    public override async Task DeleteAsync(List<ComicInfo> entities, String extra) {
        await _service!.DeleteAsync(entities, extra);
    }

}