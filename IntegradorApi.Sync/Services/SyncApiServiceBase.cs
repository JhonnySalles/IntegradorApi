using AutoMapper;
using IntegradorApi.Data.Models;

namespace IntegradorApi.Sync.Services;

/// <summary>
/// Classe base abstrata para serviços de sincronização no formato de api.
/// </summary>
/// <typeparam name="T">O tipo da entidade a ser sincronizada.</typeparam>
public abstract class SyncApiServiceBase<T> : SyncServiceBase<T> where T : Entity {

    /// <summary>
    /// Classe de mapper para conversão de DTO em Entidade ou Entidade em DTO.
    /// É protegida para que as classes filhas possam acessá-la.
    /// </summary>
    protected readonly IMapper Mapper;

    /// <summary>
    /// O construtor exige que uma conexão seja fornecida ao criar uma instância do serviço.
    /// </summary>
    /// <param name="connection">A configuração da conexão.</param>
    protected SyncApiServiceBase(Connection connection) : base(connection) {
        var config = CreateMapperInstance();
        config.AssertConfigurationIsValid();
        Mapper = config.CreateMapper();
    }

    protected abstract MapperConfiguration CreateMapperInstance();

}