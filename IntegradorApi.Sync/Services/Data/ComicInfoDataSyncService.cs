using IntegradorApi.Data.Models;
using IntegradorApi.Data.Models.ProcessaTexto;
using IntegradorApi.Data.Repositories;
using IntegradorApi.Data.Repositories.Interfaces;
using IntegradorApi.Sync.Interfaces;
using MySqlConnector;
using Serilog;

namespace IntegradorApi.Sync.Services.Data;

public class ComicInfoDataSyncService : SyncServiceBase<ComicInfo> {
    private readonly ILogger _logger;
    private IComicInfoDao _dao;

    public ComicInfoDataSyncService(Connection connection, ILogger logger) : base(connection) {
        _logger = logger;
    }

    protected override async void initialize() {
        var dbConnection = new MySqlConnection(Connection.Address);
        await dbConnection.OpenAsync();
        _dao = DaoFactory.CreateComicInfoDao(dbConnection);
    }

    public override async Task GetAsync(DateTime since, ProgressCallback<ComicInfo> onPageReceived) {
        _logger.Information("Iniciando consulta de Comic Info para a conexão {Description}", Connection.Description);

        var list = await _dao.FindForUpdateAsync(since);
        if (list == null || !list.Any()) {
            _logger.Warning("Nenhum dado encontrada para a conexão {Description}", Connection.Description);
            return;
        }

        await onPageReceived.Invoke(list, "");
    }

    public override async Task SaveAsync(List<ComicInfo> entities, String extra) {
        _logger.Information("Salvando {Count} volumes de Novels", entities.Count);

        foreach (var volume in entities)
            await _dao.SaveAsync(volume);
    }

    public override async Task DeleteAsync(List<ComicInfo> entities, String extra) {
        _logger.Information("Deletando {Count} volumes de Novels", entities.Count);

        foreach (var entity in entities)
            await _dao.DeleteAsync(entity);
    }

}