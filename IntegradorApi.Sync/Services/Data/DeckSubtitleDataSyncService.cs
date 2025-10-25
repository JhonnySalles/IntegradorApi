using IntegradorApi.Data.Models;
using IntegradorApi.Data.Models.DeckSubtitle;
using IntegradorApi.Data.Repositories;
using IntegradorApi.Data.Repositories.Interfaces;
using IntegradorApi.Sync.Interfaces;
using MySqlConnector;
using Serilog;

namespace IntegradorApi.Sync.Services.Data;

public class DeckSubtitleDataSyncService : SyncServiceBase<Subtitle> {
    private readonly ILogger _logger;
    private IDeckSubtitleDao _dao;

    public DeckSubtitleDataSyncService(Connection connection, ILogger logger) : base(connection) {
        _logger = logger;
    }

    protected override async void initialize() {
        var dbConnection = new MySqlConnection(Connection.Address);
        await dbConnection.OpenAsync();
        _dao = DaoFactory.CreateDeckSubtitleDao(dbConnection);
    }

    public override async Task GetAsync(DateTime since, ProgressCallback<Subtitle> onPageReceived) {
        _logger.Information("Iniciando consulta de Deck Subtitle para a conexão {Description}", Connection.Description);

        var tables = await _dao.GetTablesAsync(since);
        if (tables == null || !tables.Any()) {
            _logger.Warning("Nenhum dado encontrada para a conexão {Description}", Connection.Description);
            return;
        }

        foreach (var table in tables)
            await onPageReceived.Invoke(await _dao.SelectAllAsync(table, since), table);
    }

    public override async Task SaveAsync(List<Subtitle> entities, String extra) {
        _logger.Information("Salvando {Count} de itens na lista de Deck Subtitle", entities.Count);

        if (!await _dao.ExistTableAsync(extra))
            await _dao.CreateTableAsync(extra);

        foreach (var entity in entities) {
            if (await _dao.ExistAsync(extra, (Guid)entity.Id))
                await _dao.DeleteAsync(extra, entity);

            await _dao.InsertAsync(extra, entity);

        }
    }

    public override async Task DeleteAsync(List<Subtitle> entities, String extra) {
        _logger.Information("Deletando {Count} de itens na lista de Deck Subtitle", entities.Count);

        foreach (var entity in entities)
            await _dao.DeleteAsync(extra, entity);
    }

}