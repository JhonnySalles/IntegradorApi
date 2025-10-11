namespace IntegradorApi.Sync.Interfaces {
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// Delegate para callback de progresso durante a sincronização.
    /// </summary>
    /// <typeparam name="T">O tipo da entidade.</typeparam>
    public delegate Task ProgressCallback<T>(List<T> entities, String extra);

    /// <summary>
    /// Define um contrato genérico para serviços de sincronização.
    /// </summary>
    /// <typeparam name="T">O tipo da entidade a ser sincronizada, que deve herdar da classe base Entity.</typeparam>
    public interface ISyncService<T> where T : Entity {

        /// <summary>
        /// Obtem a descrição da sincronização.
        /// </summary>
        String Description();

        /// <summary>
        /// Carrega entidades da fonte de dados de forma paginada e invoca um callback para cada página recebida.
        /// </summary>
        /// <param name="since">A data da última sincronização.</param>
        /// <param name="onPageReceived">A função de callback a ser executada para cada lote de entidades.</param>
        Task GetAsync(DateTime since, ProgressCallback<T> onPageReceived);

        /// <summary>
        /// Salva uma lista de entidades no banco de dados local.
        /// </summary>
        /// <param name="entities">A lista de entidades para salvar.</param>
        /// <param name="extra">Um atributo extra para o processamento, como tabela.</param>
        Task SaveAsync(List<T> entities, String extra);

        /// <summary>
        /// Salva uma única entidade no banco de dados local.
        /// </summary>
        /// <param name="entity">A entidade para salvar.</param>
        /// <param name="extra">Um atributo extra para o processamento, como tabela.</param>
        Task SaveAsync(T entity, String extra) => SaveAsync(new List<T> { entity }, extra);

        /// <summary>
        /// Deleta uma lista de entidades do banco de dados local.
        /// </summary>
        /// <param name="entities">A lista de entidades para deletar.</param>
        /// <param name="extra">Um atributo extra para o processamento, como tabela.</param>
        Task DeleteAsync(List<T> entities, String extra);

        /// <summary>
        /// Deleta uma única entidade do banco de dados local.
        /// </summary>
        /// <param name="entity">A entidade para deletar.</param>
        /// <param name="extra">Um atributo extra para o processamento, como tabela.</param>
        Task DeleteAsync(T entity, String extra) => DeleteAsync(new List<T> { entity }, extra);
    }
}
