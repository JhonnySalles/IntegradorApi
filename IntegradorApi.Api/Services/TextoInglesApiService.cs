using IntegradorApi.Api.Core;
using IntegradorApi.Api.Models;
using IntegradorApi.Api.Models.TextoIngles;
using IntegradorApi.Data.Services;
using Serilog;
using System.Web;

namespace IntegradorApi.Api.Services;

public class TextoInglesApiService {
    private readonly ApiClientService _apiClient;
    private readonly ILogger _logger;

    public TextoInglesApiService(ApiClientService apiClient, ILogger logger) {
        _apiClient = apiClient;
        _logger = logger;
    }

    #region ExclusaoDto
    public async Task<PagedApiResponse<ExclusaoDto>?> GetUpdatesExclusaoAsync(DateTime lastUpdate, int page = 0) {
        _logger.Information("Buscando atualizações de Exclusao (TextoIngles), página {Page}...", page);
        string formattedDate = lastUpdate.ToString("yyyy-MM-ddTHH:mm:ss");
        var uriBuilder = new UriBuilder(_apiClient.GetBaseAddress()) {
            Path = $"/api/texto-ingles/exclusao/atualizacao/{formattedDate}"
        };
        var query = HttpUtility.ParseQueryString(uriBuilder.Query);
        query["page"] = page.ToString();
        query["size"] = Constants.DefaultPageSize.ToString();
        query["direction"] = "asc";
        uriBuilder.Query = query.ToString();
        return await _apiClient.GetAsync<PagedApiResponse<ExclusaoDto>>(uriBuilder.Uri.PathAndQuery);
    }

    public async Task<bool> SendAsync(List<ExclusaoDto> lista) {
        if (lista == null || !lista.Any()) return true;
        _logger.Information("Enviando {Count} atualizações de Exclusao (TextoIngles)...", lista.Count);
        return await _apiClient.PatchAsync("/api/texto-ingles/exclusao/lista", lista);
    }

    public async Task<bool> DeleteAsync(List<ExclusaoDto> lista) {
        if (lista == null || !lista.Any()) return true;
        _logger.Information("Deletando {Count} atualizações de Exclusao (TextoIngles)...", lista.Count);
        return await _apiClient.DeleteAsync("/api/texto-ingles/exclusao/lista", lista);
    }
    #endregion

    #region RevisarDto
    public async Task<PagedApiResponse<RevisarDto>?> GetUpdatesRevisarAsync(DateTime lastUpdate, int page = 0) {
        _logger.Information("Buscando atualizações de Revisar (TextoIngles), página {Page}...", page);
        string formattedDate = lastUpdate.ToString("yyyy-MM-ddTHH:mm:ss");
        var uriBuilder = new UriBuilder(_apiClient.GetBaseAddress()) {
            Path = $"/api/texto-ingles/revisar/atualizacao/{formattedDate}"
        };
        var query = HttpUtility.ParseQueryString(uriBuilder.Query);
        query["page"] = page.ToString();
        query["size"] = Constants.DefaultPageSize.ToString();
        query["direction"] = "asc";
        uriBuilder.Query = query.ToString();
        return await _apiClient.GetAsync<PagedApiResponse<RevisarDto>>(uriBuilder.Uri.PathAndQuery);
    }

    public async Task<bool> SendAsync(List<RevisarDto> lista) {
        if (lista == null || !lista.Any()) return true;
        _logger.Information("Enviando {Count} atualizações de Revisar (TextoIngles)...", lista.Count);
        return await _apiClient.PatchAsync("/api/texto-ingles/revisar/lista", lista);
    }

    public async Task<bool> DeleteAsync(List<RevisarDto> lista) {
        if (lista == null || !lista.Any()) return true;
        _logger.Information("Deletando {Count} atualizações de Revisar (TextoIngles)...", lista.Count);
        return await _apiClient.DeleteAsync("/api/texto-ingles/revisar/lista", lista);
    }
    #endregion

    #region ValidoDto
    public async Task<PagedApiResponse<ValidoDto>?> GetUpdatesValidoAsync(DateTime lastUpdate, int page = 0) {
        _logger.Information("Buscando atualizações de Valido (TextoIngles), página {Page}...", page);
        string formattedDate = lastUpdate.ToString("yyyy-MM-ddTHH:mm:ss");
        var uriBuilder = new UriBuilder(_apiClient.GetBaseAddress()) {
            Path = $"/api/texto-ingles/valido/atualizacao/{formattedDate}"
        };
        var query = HttpUtility.ParseQueryString(uriBuilder.Query);
        query["page"] = page.ToString();
        query["size"] = Constants.DefaultPageSize.ToString();
        query["direction"] = "asc";
        uriBuilder.Query = query.ToString();
        return await _apiClient.GetAsync<PagedApiResponse<ValidoDto>>(uriBuilder.Uri.PathAndQuery);
    }

    public async Task<bool> SendAsync(List<ValidoDto> lista) {
        if (lista == null || !lista.Any()) return true;
        _logger.Information("Enviando {Count} atualizações de Valido (TextoIngles)...", lista.Count);
        return await _apiClient.PatchAsync("/api/texto-ingles/valido/lista", lista);
    }

    public async Task<bool> DeleteAsync(List<ValidoDto> lista) {
        if (lista == null || !lista.Any()) return true;
        _logger.Information("Deletando {Count} atualizações de Valido (TextoIngles)...", lista.Count);
        return await _apiClient.DeleteAsync("/api/texto-ingles/valido/lista", lista);
    }
    #endregion

    #region VocabularioDto
    public async Task<PagedApiResponse<VocabularioDto>?> GetUpdatesVocabularioAsync(DateTime lastUpdate, int page = 0) {
        _logger.Information("Buscando atualizações de Vocabulario (TextoIngles), página {Page}...", page);
        string formattedDate = lastUpdate.ToString("yyyy-MM-ddTHH:mm:ss");
        var uriBuilder = new UriBuilder(_apiClient.GetBaseAddress()) {
            Path = $"/api/texto-ingles/vocabulario/atualizacao/{formattedDate}"
        };
        var query = HttpUtility.ParseQueryString(uriBuilder.Query);
        query["page"] = page.ToString();
        query["size"] = Constants.DefaultPageSize.ToString();
        query["direction"] = "asc";
        uriBuilder.Query = query.ToString();
        return await _apiClient.GetAsync<PagedApiResponse<VocabularioDto>>(uriBuilder.Uri.PathAndQuery);
    }

    public async Task<bool> SendAsync(List<VocabularioDto> lista) {
        if (lista == null || !lista.Any()) return true;
        _logger.Information("Enviando {Count} atualizações de Vocabulario (TextoIngles)...", lista.Count);
        return await _apiClient.PatchAsync("/api/texto-ingles/vocabulario/lista", lista);
    }

    public async Task<bool> DeleteAsync(List<VocabularioDto> lista) {
        if (lista == null || !lista.Any()) return true;
        _logger.Information("Deletando {Count} atualizações de Vocabulario (TextoIngles)...", lista.Count);
        return await _apiClient.DeleteAsync("/api/texto-ingles/vocabulario/lista", lista);
    }
    #endregion
}
