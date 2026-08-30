using IntegradorApi.Data.Models;
using IntegradorApi.Data.Services;
using IntegradorApi.Sync.Interfaces;
using Serilog;

namespace IntegradorApi.Sync.Services;

/// <summary>
/// Classe base abstrata para todos os serviços de sincronização.
/// Implementa a interface ISyncService e ISyncRunner, gerenciando o objeto de Conexão.
/// </summary>
/// <typeparam name="T">O tipo da entidade a ser sincronizada.</typeparam>
public abstract class SyncServiceBase<T> : ISyncService<T>, ISyncRunner where T : Entity {
    /// <summary>
    /// A conexão de origem (API) ou de destino (Banco) a ser utilizada pelo serviço.
    /// É protegida para que as classes filhas possam acessá-la.
    /// </summary>
    protected readonly Connection Connection;

    /// <summary>
    /// O construtor exige que uma conexão seja fornecida ao criar uma instância do serviço.
    /// </summary>
    /// <param name="connection">A configuração da conexão.</param>
    protected SyncServiceBase(Connection connection) {
        Connection = connection ?? throw new ArgumentNullException(nameof(connection));
        Initialize();
    }

    public Type EntityType => typeof(T);
    public virtual string ResourceName => typeof(T).Name;

    protected abstract void Initialize();

    public string Description() {
        return Connection.Description;
    }

    public abstract Task GetAsync(DateTime since, ProgressCallback<T> onPageReceived);

    public abstract Task SaveAsync(List<T> entities, string extra);

    public abstract Task DeleteAsync(List<T> entities, string extra);

    public async Task RunSyncAsync(IEnumerable<ISyncRunner> destinationServices, Connection connectionOrigin, DatabaseService databaseService, ILogger logger) {
        var matchingDestinations = destinationServices.OfType<ISyncService<T>>().ToList();
        if (!matchingDestinations.Any()) {
            return;
        }

        logger.Information("Processando sincronização: {Description} ({ResourceName})", connectionOrigin.Description, ResourceName);

        DateTime lastSyncDate = await databaseService.GetLastSyncDateAsync(connectionOrigin.Id, ResourceName);
        DateTime syncTime = DateTime.UtcNow;

        async Task HandlePage(List<T> pageToSave, string extra) {
            foreach (var destinationService in matchingDestinations) {
                logger.Information("Enviando registros ({Count}) para {Description}", pageToSave.Count, destinationService.Description());
                await destinationService.SaveAsync(pageToSave, extra);
            }

            if (connectionOrigin.Delete) {
                logger.Information("Deletando registros ({Count}) de {Description}", pageToSave.Count, Description());
                await DeleteAsync(pageToSave, extra);
            }
        }

        await GetAsync(lastSyncDate, HandlePage);
        await databaseService.UpdateLastSyncDateAsync(connectionOrigin.Id, ResourceName, syncTime);

        logger.Information("--------------------------------------------------------------------------------");
    }
}