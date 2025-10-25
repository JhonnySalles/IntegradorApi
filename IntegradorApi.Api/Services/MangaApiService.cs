using IntegradorApi.Api.Core;
using IntegradorApi.Api.Models;
using IntegradorApi.Data.Services;
using Serilog;
using System.Web;

namespace IntegradorApi.Api.Services;

public class MangaApiService {
    private readonly ApiClientService _apiClient;
    private readonly ILogger _logger;

    public MangaApiService(ApiClientService apiClient, ILogger logger) {
        _apiClient = apiClient;
        _logger = logger;
    }

    /// <summary>
    /// Busca a lista de tabelas de manga disponíveis na API.
    /// </summary>
    /// <returns>A lista de tabelas disponível.</returns>
    public async Task<List<string>?> GetTablesAsync() {
        _logger.Information("Buscando lista de tabelas de Manga...");
        return await _apiClient.GetAsync<List<string>>("/api/manga-extractor/tabelas");
    }

    /// <summary>
    /// Busca atualizações de volumes de uma tabela específica de forma paginada.
    /// </summary>
    /// <param name="tableName">O nome da tabela para consultar.</param>
    /// <param name="lastUpdate">A data da última atualização para buscar registros mais novos.</param>
    /// <param name="page">O número da página a ser buscada (começa em 0).</param>
    /// <returns>A lista paginada da entidade solicitada.</returns>
    public async Task<PagedApiResponse<MangaVolumeDto>?> GetUpdatesAsync(string tableName, DateTime lastUpdate, int page = 0) {
        _logger.Information("Buscando atualizações para a tabela {TableName}, página {Page}...", tableName, page);

        string formattedDate = lastUpdate.ToString("yyyy-MM-ddTHH:mm:ss");

        var uriBuilder = new UriBuilder(_apiClient.GetBaseAddress()) {
            Path = $"/api/manga-extractor/tabela/{tableName}/atualizacao/{formattedDate}"
        };
        var query = HttpUtility.ParseQueryString(uriBuilder.Query);
        query["page"] = page.ToString();
        query["size"] = Constants.DefaultPageSize.ToString();
        query["direction"] = "asc";
        uriBuilder.Query = query.ToString();

        string requestUri = uriBuilder.Uri.PathAndQuery;

        return await _apiClient.GetAsync<PagedApiResponse<MangaVolumeDto>>(requestUri);
    }

    /// <summary>
    /// Envia uma lista de volumes de novel para a API para serem criados ou atualizados.
    /// </summary>
    /// <param name="tableName">O nome da tabela de destino na API.</param>
    /// <param name="volumes">A lista de objetos MangaVolumeDto a ser enviada.</param>
    /// <returns>Verdadeiro se o envio foi bem-sucedido.</returns>
    public async Task<bool> SendVolumesAsync(string tableName, List<MangaVolumeDto> volumes) {
        if (volumes == null || !volumes.Any()) {
            _logger.Information("Nenhum volume de Manga para enviar para a tabela {TableName}.", tableName);
            return true;
        }

        _logger.Information("Enviando {VolumeCount} volumes de Manga para a tabela {TableName}...", volumes.Count, tableName);

        var requestUri = $"/api/manga-extractor/tabela/{tableName}/lista";

        return await _apiClient.PatchAsync(requestUri, volumes);
    }

    /// <summary>
    /// Deleta uma lista de volumes de uma tabela específica.
    /// </summary>
    /// <param name="tableName">O nome da tabela para deletar.</param>
    /// <param name="volumes">A lista de objetos MangaVolumeDto a ser deletado.</param>
    /// <returns>Verdadeiro se o delete foi bem-sucedido.</returns>
    public async Task<bool> DeleteVolumesAsync(string tableName, List<MangaVolumeDto> volumes) {
        if (volumes == null || !volumes.Any()) {
            _logger.Information("Nenhum volume de mangá para deletar da tabela {TableName}.", tableName);
            return true;
        }

        _logger.Information("Deletando {VolumeCount} volumes de Manga para a tabela {TableName}...", volumes.Count, tableName);

        var requestUri = $"/api/manga-extractor/tabela/{tableName}/lista";

        return await _apiClient.DeleteAsync(requestUri, volumes);
    }
}