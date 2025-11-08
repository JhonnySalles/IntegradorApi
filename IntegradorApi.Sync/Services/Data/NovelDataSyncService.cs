using IntegradorApi.Data.Models;
using IntegradorApi.Data.Models.NovelExtractor;
using IntegradorApi.Data.Repositories;
using IntegradorApi.Data.Repositories.Interfaces;
using IntegradorApi.Sync.Interfaces;
using MySqlConnector;
using Serilog;

namespace IntegradorApi.Sync.Services.Data;

public class NovelDataSyncService : SyncDataServiceBase<NovelVolume> {
    private readonly ILogger _logger;
    private INovelExtractorDao? _dao;

    public NovelDataSyncService(Connection connection, ILogger logger) : base(connection) {
        _logger = logger;
    }

    protected override async void Initialize() {
        var dbConnection = new MySqlConnection(Connection.Address);
        await dbConnection.OpenAsync();
        _dao = DaoFactory.CreateNovelExtractorDao(dbConnection, Connection.Optional);
    }

    public override async Task GetAsync(DateTime since, ProgressCallback<NovelVolume> onPageReceived) {
        _logger.Information("Iniciando consulta de Novels para a conexão {Description}", Connection.Description);

        var tables = await _dao!.GetTablesAsync(since);
        if (tables == null || !tables.Any()) {
            _logger.Warning("Nenhum dado encontrada para a conexão {Description}", Connection.Description);
            return;
        }

        foreach (var table in tables)
            await onPageReceived.Invoke(await _dao.SelectAllVolumesAsync(table, since), table);
    }

    public override async Task SaveAsync(List<NovelVolume> entities, String extra) {
        _logger.Information("Salvando {Count} volumes de Novels", entities.Count);

        if (!await _dao!.ExistTableAsync(extra))
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

    public override async Task DeleteAsync(List<NovelVolume> entities, String extra) {
        _logger.Information("Deletando {Count} volumes de Novels", entities.Count);

        foreach (var entity in entities)
            await _dao!.DeleteVolumeAsync(extra, entity);
    }

}