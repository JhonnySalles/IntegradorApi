using IntegradorApi.Data.Models;
using IntegradorApi.Data.Models.MangaExtractor;
using IntegradorApi.Data.Repositories; // Para o DaoFactory
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
        initialize();
    }

    private async void initialize() {
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
            onPageReceived.Invoke(await _dao.SelectAllVolumesAsync(table, since));
    }

    public override async Task SaveAsync(List<MangaVolume> entities) {
        _logger.Information("Salvando {Count} volumes de Mangas", entities.Count);

        var table = "";
        foreach (var volume in entities) {
            if (table != volume.Tabela) {
                table = volume.Tabela;
                if (!await _dao.ExistTableAsync(table))
                    await _dao.CreateTableAsync(table);
            }

            if (await _dao.ExistVolumeAsync(table, (Guid)volume.Id))
                await _dao.DeleteVolumeAsync(table, volume);

            await _dao.InsertVolumeAsync(table, volume);
            foreach (var capitulo in volume.Capitulos) {
                await _dao.InsertCapituloAsync(table, (Guid)volume.Id, capitulo);
                foreach (var pagina in capitulo.Paginas) {
                    await _dao.InsertPaginaAsync(table, (Guid)capitulo.Id, pagina);
                    foreach (var texto in pagina.Textos)
                        await _dao.InsertTextoAsync(table, (Guid)pagina.Id, texto);
                }
            }
        }
    }

    public override async Task DeleteAsync(List<MangaVolume> entities) {
        _logger.Information("Deletando {Count} volumes de Mangas", entities.Count);

        foreach (var entity in entities)
            await _dao.DeleteVolumeAsync(entity.Tabela, entity);
    }
}