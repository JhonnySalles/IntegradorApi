using IntegradorApi.Data.Models.DeckSubtitle;
using IntegradorApi.Data.Repositories;
using IntegradorApi.Data.Repositories.Interfaces;
using MySqlConnector;
using Serilog;

namespace IntegradorApi.Api.Services;

public class DeckSubtitleDataService {
    private readonly IDeckSubtitleDao _dao;
    private readonly ILogger _logger;

    public DeckSubtitleDataService(MySqlConnection connection, ILogger logger) {
        _dao = DaoFactory.CreateDeckSubtitleDao(connection);
        _logger = logger;
    }

    public async Task<List<string>?> GetTablesAsync(DateTime since) {
        return await _dao.GetTablesAsync(since);
    }

    public async Task<List<Subtitle>> SelectAllAsync(string table, DateTime since) {
        return await _dao.SelectAllAsync(table, since);
    }

    public async Task SaveAsync(List<Subtitle> entities, String extra) {
        _logger.Information("Salvando {Count} de itens na lista de Deck Subtitle", entities.Count);

        if (!await _dao.ExistTableAsync(extra))
            await _dao.CreateTableAsync(extra);

        foreach (var entity in entities) {
            if (await _dao.ExistAsync(extra, entity.Id))
                await _dao.DeleteAsync(extra, entity);

            await _dao.InsertAsync(extra, entity);

        }
    }

    public async Task DeleteAsync(List<Subtitle> entities, String extra) {
        _logger.Information("Deletando {Count} de itens na lista de Deck Subtitle", entities.Count);

        foreach (var entity in entities)
            await _dao.DeleteAsync(extra, entity);
    }
}