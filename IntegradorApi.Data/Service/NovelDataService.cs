using IntegradorApi.Data.Models.NovelExtractor;
using IntegradorApi.Data.Repositories;
using IntegradorApi.Data.Repositories.Interfaces;
using MySqlConnector;
using Serilog;

namespace IntegradorApi.Api.Services;

public class NovelDataService {
    private readonly INovelExtractorDao _dao;
    private readonly ILogger _logger;

    public NovelDataService(MySqlConnection connection, string optional, ILogger logger) {
        _logger = logger;
        _dao = DaoFactory.CreateNovelExtractorDao(connection, optional);
    }

    public async Task<List<string>?> GetTablesAsync(DateTime since) {
        return await _dao.GetTablesAsync(since);
    }

    public async Task<List<NovelVolume>> SelectAllVolumesAsync(string table, DateTime since) {
        return await _dao.SelectAllVolumesAsync(table, since);
    }

    public async Task SaveAsync(List<NovelVolume> entities, String extra) {
        _logger.Information("Salvando {Count} volumes de Novels", entities.Count);

        if (!await _dao.ExistTableAsync(extra))
            await _dao.CreateTableAsync(extra);

        foreach (var volume in entities) {
            if (await _dao.ExistVolumeAsync(extra, (Guid)volume.Id))
                await _dao.DeleteVolumeAsync(extra, volume);

            await _dao.InsertVolumeAsync(extra, volume);
            foreach (var capitulo in volume.Capitulos) {
                await _dao.InsertCapituloAsync(extra, (Guid)volume.Id, capitulo);
                foreach (var texto in capitulo.Textos)
                    await _dao.InsertTextoAsync(extra, (Guid)capitulo.Id, texto);

            }
        }
    }

    public async Task DeleteAsync(List<NovelVolume> entities, String extra) {
        _logger.Information("Deletando {Count} volumes de Novels", entities.Count);

        foreach (var entity in entities)
            await _dao.DeleteVolumeAsync(extra, entity);
    }
}