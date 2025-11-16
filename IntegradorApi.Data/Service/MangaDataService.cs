using IntegradorApi.Data.Models.MangaExtractor;
using IntegradorApi.Data.Repositories;
using IntegradorApi.Data.Repositories.Interfaces;
using MySqlConnector;
using Serilog;

namespace IntegradorApi.Api.Services;

public class MangaDataService {
    private readonly IMangaExtractorDao _dao;
    private readonly ILogger _logger;

    public MangaDataService(MySqlConnection connection, string optional, ILogger logger) {
        _logger = logger;
        _dao = DaoFactory.CreateMangaExtractorDao(connection, optional);
    }

    public async Task<List<string>?> GetTablesAsync(DateTime since) {
        return await _dao.GetTablesAsync(since);
    }

    public async Task<List<MangaVolume>> SelectAllVolumesAsync(string table, DateTime since) {
        return await _dao.SelectAllVolumesAsync(table, since);
    }

    public async Task SaveAsync(List<MangaVolume> entities, String extra) {
        _logger.Information("Salvando {Count} volumes de Mangas", entities.Count);

        if (!await _dao.ExistTableAsync(extra))
            await _dao.CreateTableAsync(extra);

        foreach (var volume in entities) {
            if (await _dao.ExistVolumeAsync(extra, (Guid)volume.Id))
                await _dao.DeleteVolumeAsync(extra, volume);

            await _dao.InsertVolumeAsync(extra, volume);
            foreach (var capitulo in volume.Capitulos) {
                await _dao.InsertCapituloAsync(extra, (Guid)volume.Id, capitulo);
                foreach (var pagina in capitulo.Paginas) {
                    await _dao.InsertPaginaAsync(extra, (Guid)capitulo.Id, pagina);
                    foreach (var texto in pagina.Textos)
                        await _dao.InsertTextoAsync(extra, (Guid)pagina.Id, texto);
                }
            }
        }
    }

    public async Task DeleteAsync(List<MangaVolume> entities, String extra) {
        _logger.Information("Deletando {Count} volumes de Mangas", entities.Count);

        foreach (var entity in entities)
            await _dao.DeleteVolumeAsync(extra, entity);
    }
}