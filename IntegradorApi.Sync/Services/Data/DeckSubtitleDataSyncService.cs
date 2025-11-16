using IntegradorApi.Api.Services;
using IntegradorApi.Data.Models;
using IntegradorApi.Data.Models.DeckSubtitle;
using IntegradorApi.Sync.Interfaces;
using MySqlConnector;
using Serilog;

namespace IntegradorApi.Sync.Services.Data;

public class DeckSubtitleDataSyncService : SyncDataServiceBase<Subtitle> {
    private readonly ILogger _logger;
    private DeckSubtitleDataService? _service;

    public DeckSubtitleDataSyncService(Connection connection, ILogger logger) : base(connection) {
        _logger = logger;
    }

    protected override async void Initialize() {
        var dbConnection = new MySqlConnection(Connection.Address);
        await dbConnection.OpenAsync();
        _service = new DeckSubtitleDataService(dbConnection, _logger);
    }

    public override async Task GetAsync(DateTime since, ProgressCallback<Subtitle> onPageReceived) {
        _logger.Information("Iniciando consulta de Deck Subtitle para a conexão {Description}", Connection.Description);

        var tables = await _service!.GetTablesAsync(since);
        if (tables == null || !tables.Any()) {
            _logger.Warning("Nenhum dado encontrada para a conexão {Description}", Connection.Description);
            return;
        }

        foreach (var table in tables)
            await onPageReceived.Invoke(await _service.SelectAllAsync(table, since), table);
    }

    public override async Task SaveAsync(List<Subtitle> entities, String extra) {
        await _service!.SaveAsync(entities, extra);
    }

    public override async Task DeleteAsync(List<Subtitle> entities, String extra) {
        await _service!.DeleteAsync(entities, extra);
    }

}