using IntegradorApi.Api.Services;
using IntegradorApi.Data.Models;
using IntegradorApi.Data.Models.MangaExtractor;
using IntegradorApi.Sync.Interfaces;
using MySqlConnector;
using Serilog;

namespace IntegradorApi.Sync.Services.Data;

public class MangaDataSyncService : SyncDataServiceBase<MangaVolume> {
    private readonly ILogger _logger;
    private MangaDataService? _service;

    public MangaDataSyncService(Connection connection, ILogger logger) : base(connection) {
        _logger = logger;
    }

    protected override async void Initialize() {
        var dbConnection = new MySqlConnection(Connection.Address);
        await dbConnection.OpenAsync();
        _service = new MangaDataService(dbConnection, Connection.Optional, _logger);
    }

    public override async Task GetAsync(DateTime since, ProgressCallback<MangaVolume> onPageReceived) {
        _logger.Information("Iniciando consulta de Mangas para a conexão {Description}", Connection.Description);

        var tables = await _service!.GetTablesAsync(since);
        if (tables == null || !tables.Any()) {
            _logger.Warning("Nenhum dado encontrada para a conexão {Description}", Connection.Description);
            return;
        }

        foreach (var table in tables)
            await onPageReceived.Invoke(await _service.SelectAllVolumesAsync(table, since), table);
    }

    public override async Task SaveAsync(List<MangaVolume> entities, String extra) {
        await _service!.SaveAsync(entities, extra);
    }

    public override async Task DeleteAsync(List<MangaVolume> entities, String extra) {
        await _service!.DeleteAsync(entities, extra);
    }

}