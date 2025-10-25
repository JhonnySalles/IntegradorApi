using IntegradorApi.Api.Core;
using IntegradorApi.Api.Models;
using IntegradorApi.Data.Services;
using Serilog;
using System.Web;

namespace IntegradorApi.Api.Services;

public class ComicInfoApiService {
    private readonly ApiClientService _apiClient;
    private readonly ILogger _logger;

    public ComicInfoApiService(ApiClientService apiClient, ILogger logger) {
        _apiClient = apiClient;
        _logger = logger;
    }

    /// <summary>
    /// Busca atualizações de Comic Info de forma paginada.
    /// </summary>
    /// <param name="lastUpdate">A data da última atualização para buscar registros mais novos.</param>
    /// <param name="page">O número da página a ser buscada (começa em 0).</param>
    /// <returns>A lista paginada da entidade solicitada.</returns>
    public async Task<PagedApiResponse<ComicInfoDto>?> GetUpdatesAsync(DateTime lastUpdate, int page = 0) {
        _logger.Information("Buscando atualizações do Comic Info, página {Page}...", page);

        string formattedDate = lastUpdate.ToString("yyyy-MM-ddTHH:mm:ss");

        var uriBuilder = new UriBuilder(_apiClient.GetBaseAddress()) {
            Path = $"/api/comic-info/atualizacao/{formattedDate}"
        };
        var query = HttpUtility.ParseQueryString(uriBuilder.Query);
        query["page"] = page.ToString();
        query["size"] = Constants.DefaultPageSize.ToString();
        query["direction"] = "asc";
        uriBuilder.Query = query.ToString();

        string requestUri = uriBuilder.Uri.PathAndQuery;

        return await _apiClient.GetAsync<PagedApiResponse<ComicInfoDto>>(requestUri);
    }

    /// <summary>
    /// Envia uma lista de Comic Info para a API para serem criados ou atualizados.
    /// </summary>
    /// <param name="lista">A lista de objetos ComicInfoDto a ser enviada.</param>
    /// <returns>Verdadeiro se o envio foi bem-sucedido.</returns>
    public async Task<bool> SendAsync(List<ComicInfoDto> lista) {
        if (lista == null || !lista.Any()) {
            _logger.Information("Nenhuma atualização de Comic Info para enviar.");
            return true;
        }

        _logger.Information("Enviando {VolumeCount} atualização de Comic Info...", lista.Count);
        var requestUri = $"/api/comic-info/lista";
        return await _apiClient.PatchAsync(requestUri, lista);
    }

    /// <summary>
    /// Deleta uma lista de Comic Info.
    /// </summary>
    /// <param name="lista">A lista de objetos ComicInfoDto a ser deletado.</param>
    /// <returns>Verdadeiro se o delete foi bem-sucedido.</returns>
    public async Task<bool> DeleteAsync(List<ComicInfoDto> lista) {
        if (lista == null || !lista.Any()) {
            _logger.Information("Nenhum atualização de Comic Info para deletar.");
            return true;
        }

        _logger.Information("Deletando {Count} de atualização de Comic Info...", lista.Count);
        var requestUri = $"/api/comic-info/lista";
        return await _apiClient.DeleteAsync(requestUri, lista);
    }
}