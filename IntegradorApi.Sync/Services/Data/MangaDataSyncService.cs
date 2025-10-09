using IntegradorApi.Data.Models;
using IntegradorApi.Data.Models.MangaExtractor;
using IntegradorApi.Data.Repositories;
using IntegradorApi.Data.Repositories.Interfaces;
using IntegradorApi.Sync.Interfaces;
using MySqlConnector;
using Serilog;

namespace IntegradorApi.Sync.Services.Data;

public class MangaDataSyncService : SyncServiceBase<MangaVolume> {
    private readonly ILogger _logger;
    private IMangaExtractorDao _dao;

    public MangaDataSyncService(Connection connection, ILogger logger) : base(connection) {
        _logger = logger;
    }

    protected override async void initialize() {
        var dbConnection = new MySqlConnection(Connection.Address);
        await dbConnection.OpenAsync();
        _dao = DaoFactory.CreateMangaExtractorDao(dbConnection, Connection.Optional);
    }

    public override async Task GetAsync(DateTime since, ProgressCallback<MangaVolume> onPageReceived) {
        _logger.Information("Iniciando consulta de Mangás para a conexão {Description}", Connection.Description);

        var tables = await _dao.GetTablesAsync(since);
        if (tables == null || !tables.Any()) {
            _logger.Warning("Nenhum dado encontrada para a conexão {Description}", Connection.Description);
            return;
        }

        foreach (var table in tables)
            await onPageReceived.Invoke(await _dao.SelectAllVolumesAsync(table, since), table);
    }

    public override async Task SaveAsync(List<MangaVolume> entities, String extra) {
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

    public override async Task DeleteAsync(List<MangaVolume> entities, String extra) {
        _logger.Information("Deletando {Count} volumes de Mangas", entities.Count);

        foreach (var entity in entities)
            await _dao.DeleteVolumeAsync(extra, entity);
    }
}