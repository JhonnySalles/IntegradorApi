using IntegradorApi.Data.Models;
using IntegradorApi.Sync.Interfaces;

namespace IntegradorApi.Sync.Services;

/// <summary>
/// Classe base abstrata para todos os serviços de sincronização.
/// Implementa a interface ISyncService e gerencia o objeto de Conexão.
/// </summary>
/// <typeparam name="T">O tipo da entidade a ser sincronizada.</typeparam>
public abstract class SyncServiceBase<T> : ISyncService<T> where T : Entity {
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
        initialize();
    }

    protected abstract void initialize();

    public abstract Task GetAsync(DateTime since, ProgressCallback<T> onPageReceived);

    public abstract Task SaveAsync(List<T> entities, String extra);

    public abstract Task DeleteAsync(List<T> entities, String extra);
}