using IntegradorApi.Data.Models.ProcessaTexto;
using IntegradorApi.Data.Repositories;
using IntegradorApi.Data.Repositories.Interfaces;
using MySqlConnector;
using Serilog;

namespace IntegradorApi.Data.Services;

public class ComicInfoDataService {
    private readonly IComicInfoDao _dao;
    private readonly ILogger _logger;

    public ComicInfoDataService(MySqlConnection connection, ILogger logger) {
        _dao = DaoFactory.CreateComicInfoDao(connection); ;
        _logger = logger;
    }

    public async Task<List<ComicInfo>?> GetAsync(DateTime since) {
        return await _dao.FindForUpdateAsync(since);
    }

    public async Task SaveAsync(List<ComicInfo> entities, String extra) {
        _logger.Information("Salvando {Count} volumes de Novels", entities.Count);

        foreach (var volume in entities)
            await _dao.SaveAsync(volume);
    }

    public async Task DeleteAsync(List<ComicInfo> entities, String extra) {
        _logger.Information("Deletando {Count} volumes de Novels", entities.Count);

        foreach (var entity in entities)
            await _dao.DeleteAsync(entity);
    }
}