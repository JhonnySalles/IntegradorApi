using IntegradorApi.Api.Core;
using IntegradorApi.Api.Models;
using IntegradorApi.Data.Services;
using Serilog;
using System.Web;

namespace IntegradorApi.Api.Services;

public class DeckSubtitleApiService {
    private readonly ApiClientService _apiClient;
    private readonly ILogger _logger;

    public DeckSubtitleApiService(ApiClientService apiClient, ILogger logger) {
        _apiClient = apiClient;
        _logger = logger;
    }

    /// <summary>
    /// Busca a lista de tabelas do Deck Subtitle disponíveis na API.
    /// </summary>
    /// <returns>A lista de tabelas disponível.</returns>
    public async Task<List<string>?> GetTablesAsync() {
        _logger.Information("Buscando lista de tabelas do Deck Subtitle...");
        return await _apiClient.GetAsync<List<string>>("/api/deck-subtitle/tabelas");
    }

    /// <summary>
    /// Busca atualizações no Deck Subtitle de uma tabela específica de forma paginada.
    /// </summary>
    /// <param name="tableName">O nome da tabela para consultar.</param>
    /// <param name="lastUpdate">A data da última atualização para buscar registros mais novos.</param>
    /// <param name="page">O número da página a ser buscada (começa em 0).</param>
    /// <returns>A lista paginada da entidade solicitada.</returns>
    public async Task<PagedApiResponse<SubtitleDto>?> GetUpdatesAsync(string tableName, DateTime lastUpdate, int page = 0) {
        _logger.Information("Buscando atualizações para a tabela {TableName}, página {Page}...", tableName, page);

        string formattedDate = lastUpdate.ToString("yyyy-MM-ddTHH:mm:ss");

        var uriBuilder = new UriBuilder(_apiClient.GetBaseAddress()) {
            Path = $"/api/deck-subtitle/tabela/{tableName}/atualizacao/{formattedDate}"
        };
        var query = HttpUtility.ParseQueryString(uriBuilder.Query);
        query["page"] = page.ToString();
        query["size"] = Constants.DefaultPageSize.ToString();
        query["direction"] = "asc";
        uriBuilder.Query = query.ToString();

        string requestUri = uriBuilder.Uri.PathAndQuery;

        return await _apiClient.GetAsync<PagedApiResponse<SubtitleDto>>(requestUri);
    }

    /// <summary>
    /// Envia uma lista de Deck Subtitle para a API para serem criados ou atualizados.
    /// </summary>
    /// <param name="tableName">O nome da tabela de destino na API.</param>
    /// <param name="list">A lista de objetos SubtitleDto a ser enviada.</param>
    /// <returns>Verdadeiro se o envio foi bem-sucedido.</returns>
    public async Task<bool> SendVolumesAsync(string tableName, List<SubtitleDto> list) {
        if (list == null || !list.Any()) {
            _logger.Information("Nenhuma atualização de Deck Subtitle para enviar para a tabela {TableName}.", tableName);
            return true;
        }

        _logger.Information("Enviando {Count} de atualização de Deck Subtitle para a tabela {TableName}...", list.Count, tableName);

        var requestUri = $"/api/deck-subtitle/tabela/{tableName}/lista";

        return await _apiClient.PatchAsync(requestUri, list);
    }

    /// <summary>
    /// Deleta uma lista de Deck Subtitle de uma tabela específica.
    /// </summary>
    /// <param name="tableName">O nome da tabela para deletar.</param>
    /// <param name="list">A lista de objetos SubtitleDto a ser deletado.</param>
    /// <returns>Verdadeiro se o delete foi bem-sucedido.</returns>
    public async Task<bool> DeleteVolumesAsync(string tableName, List<SubtitleDto> list) {
        if (list == null || !list.Any()) {
            _logger.Information("Nenhuma atualização de Deck Subtitle para deletar da tabela {TableName}.", tableName);
            return true;
        }

        _logger.Information("Deletando {Count} de atualização de Deck Subtitle para a tabela {TableName}...", list.Count, tableName);

        var requestUri = $"/api/deck-subtitle/tabela/{tableName}/lista";

        return await _apiClient.DeleteAsync(requestUri, list);
    }
}