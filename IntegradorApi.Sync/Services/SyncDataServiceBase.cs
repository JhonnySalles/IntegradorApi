using IntegradorApi.Data.Models;

namespace IntegradorApi.Sync.Services;

/// <summary>
/// Classe base abstrata para serviços de sincronização no formato de database.
/// </summary>
/// <typeparam name="T">O tipo da entidade a ser sincronizada.</typeparam>
public abstract class SyncDataServiceBase<T> : SyncServiceBase<T> where T : Entity {

    /// <summary>
    /// O construtor exige que uma conexão seja fornecida ao criar uma instância do serviço.
    /// </summary>
    /// <param name="connection">A configuração da conexão.</param>
    protected SyncDataServiceBase(Connection connection) : base(connection) {

    }

}