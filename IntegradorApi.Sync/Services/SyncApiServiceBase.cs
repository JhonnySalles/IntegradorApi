using System.Collections.Concurrent;
using AutoMapper;
using IntegradorApi.Data.Models;

namespace IntegradorApi.Sync.Services;

/// <summary>
/// Classe base abstrata para serviços de sincronização no formato de api.
/// Cacheia a instância do IMapper por tipo de serviço para evitar re-validação e re-compilação a cada ciclo.
/// </summary>
/// <typeparam name="T">O tipo da entidade a ser sincronizada.</typeparam>
public abstract class SyncApiServiceBase<T> : SyncServiceBase<T> where T : Entity {
    private static readonly ConcurrentDictionary<Type, IMapper> MapperCache = new();

    /// <summary>
    /// Instância do mapper para conversão entre DTO e Entidade.
    /// </summary>
    protected readonly IMapper Mapper;

    /// <summary>
    /// O construtor exige que uma conexão seja fornecida ao criar uma instância do serviço.
    /// </summary>
    /// <param name="connection">A configuração da conexão.</param>
    protected SyncApiServiceBase(Connection connection) : base(connection) {
        Mapper = MapperCache.GetOrAdd(GetType(), _ => {
            var config = CreateMapperInstance();
            config.AssertConfigurationIsValid();
            return config.CreateMapper();
        });
    }

    protected abstract MapperConfiguration CreateMapperInstance();
}