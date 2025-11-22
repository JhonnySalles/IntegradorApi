using IntegradorApi.Api.Core;
using IntegradorApi.Api.Models;
using IntegradorApi.Data.Services;
using Serilog;
using System.Web;

namespace IntegradorApi.Api.Services;

public class TextoJaponesApiService {
    private readonly ApiClientService _apiClient;
    private readonly ILogger _logger;

    public TextoJaponesApiService(ApiClientService apiClient, ILogger logger) {
        _apiClient = apiClient;
        _logger = logger;
    }

    #region EstatisticaDto
    public async Task<PagedApiResponse<EstatisticaDto>?> GetUpdatesEstatisticaAsync(DateTime lastUpdate, int page = 0) {
        _logger.Information("Buscando atualizações de Estatistica (TextoJapones), página {Page}...", page);
        string formattedDate = lastUpdate.ToString("yyyy-MM-ddTHH:mm:ss");
        var uriBuilder = new UriBuilder(_apiClient.GetBaseAddress()) {
            Path = $"/api/texto-japones/estatistica/atualizacao/{formattedDate}"
        };
        var query = HttpUtility.ParseQueryString(uriBuilder.Query);
        query["page"] = page.ToString();
        query["size"] = Constants.DefaultPageSize.ToString();
        query["direction"] = "asc";
        uriBuilder.Query = query.ToString();
        return await _apiClient.GetAsync<PagedApiResponse<EstatisticaDto>>(uriBuilder.Uri.PathAndQuery);
    }

    public async Task<bool> SendAsync(List<EstatisticaDto> lista) {
        if (lista == null || !lista.Any()) return true;
        _logger.Information("Enviando {Count} atualizações de Estatistica (TextoJapones)...", lista.Count);
        return await _apiClient.PatchAsync("/api/texto-japones/estatistica/lista", lista);
    }

    public async Task<bool> DeleteAsync(List<EstatisticaDto> lista) {
        if (lista == null || !lista.Any()) return true;
        _logger.Information("Deletando {Count} atualizações de Estatistica (TextoJapones)...", lista.Count);
        return await _apiClient.DeleteAsync("/api/texto-japones/estatistica/lista", lista);
    }
    #endregion

    #region ExclusaoDto
    public async Task<PagedApiResponse<ExclusaoDto>?> GetUpdatesExclusaoAsync(DateTime lastUpdate, int page = 0) {
        _logger.Information("Buscando atualizações de Exclusao (TextoJapones), página {Page}...", page);
        string formattedDate = lastUpdate.ToString("yyyy-MM-ddTHH:mm:ss");
        var uriBuilder = new UriBuilder(_apiClient.GetBaseAddress()) {
            Path = $"/api/texto-japones/exclusao/atualizacao/{formattedDate}"
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
        _logger.Information("Enviando {Count} atualizações de Exclusao (TextoJapones)...", lista.Count);
        return await _apiClient.PatchAsync("/api/texto-japones/exclusao/lista", lista);
    }

    public async Task<bool> DeleteAsync(List<ExclusaoDto> lista) {
        if (lista == null || !lista.Any()) return true;
        _logger.Information("Deletando {Count} atualizações de Exclusao (TextoJapones)...", lista.Count);
        return await _apiClient.DeleteAsync("/api/texto-japones/exclusao/lista", lista);
    }
    #endregion

    #region KanjaxDto
    public async Task<PagedApiResponse<KanjaxPtDto>?> GetUpdatesKanjaxDtoAsync(DateTime lastUpdate, int page = 0) {
        _logger.Information("Buscando atualizações de KanjaxDto (TextoJapones), página {Page}...", page);
        string formattedDate = lastUpdate.ToString("yyyy-MM-ddTHH:mm:ss");
        var uriBuilder = new UriBuilder(_apiClient.GetBaseAddress()) {
            Path = $"/api/texto-japones/kanjax-pt/atualizacao/{formattedDate}"
        };
        var query = HttpUtility.ParseQueryString(uriBuilder.Query);
        query["page"] = page.ToString();
        query["size"] = Constants.DefaultPageSize.ToString();
        query["direction"] = "asc";
        uriBuilder.Query = query.ToString();
        return await _apiClient.GetAsync<PagedApiResponse<KanjaxPtDto>>(uriBuilder.Uri.PathAndQuery);
    }

    public async Task<bool> SendAsync(List<KanjaxPtDto> lista) {
        if (lista == null || !lista.Any()) return true;
        _logger.Information("Enviando {Count} atualizações de KanjaxDto (TextoJapones)...", lista.Count);
        return await _apiClient.PatchAsync("/api/texto-japones/kanjax-pt/lista", lista);
    }

    public async Task<bool> DeleteAsync(List<KanjaxPtDto> lista) {
        if (lista == null || !lista.Any()) return true;
        _logger.Information("Deletando {Count} atualizações de KanjaxDto (TextoJapones)...", lista.Count);
        return await _apiClient.DeleteAsync("/api/texto-japones/kanjax-pt/lista", lista);
    }
    #endregion

    #region KanjiInfoDto
    public async Task<PagedApiResponse<KanjiInfoDto>?> GetUpdatesKanjiInfoAsync(DateTime lastUpdate, int page = 0) {
        _logger.Information("Buscando atualizações de KanjiInfo (TextoJapones), página {Page}...", page);
        string formattedDate = lastUpdate.ToString("yyyy-MM-ddTHH:mm:ss");
        var uriBuilder = new UriBuilder(_apiClient.GetBaseAddress()) {
            Path = $"/api/texto-japones/kanji-info/atualizacao/{formattedDate}"
        };
        var query = HttpUtility.ParseQueryString(uriBuilder.Query);
        query["page"] = page.ToString();
        query["size"] = Constants.DefaultPageSize.ToString();
        query["direction"] = "asc";
        uriBuilder.Query = query.ToString();
        return await _apiClient.GetAsync<PagedApiResponse<KanjiInfoDto>>(uriBuilder.Uri.PathAndQuery);
    }

    public async Task<bool> SendAsync(List<KanjiInfoDto> lista) {
        if (lista == null || !lista.Any()) return true;
        _logger.Information("Enviando {Count} atualizações de KanjiInfo (TextoJapones)...", lista.Count);
        return await _apiClient.PatchAsync("/api/texto-japones/kanji-info/lista", lista);
    }

    public async Task<bool> DeleteAsync(List<KanjiInfoDto> lista) {
        if (lista == null || !lista.Any()) return true;
        _logger.Information("Deletando {Count} atualizações de KanjiInfo (TextoJapones)...", lista.Count);
        return await _apiClient.DeleteAsync("/api/texto-japones/kanji-info/lista", lista);
    }
    #endregion

    #region RevisarDto
    public async Task<PagedApiResponse<RevisarDto>?> GetUpdatesRevisarAsync(DateTime lastUpdate, int page = 0) {
        _logger.Information("Buscando atualizações de Revisar (TextoJapones), página {Page}...", page);
        string formattedDate = lastUpdate.ToString("yyyy-MM-ddTHH:mm:ss");
        var uriBuilder = new UriBuilder(_apiClient.GetBaseAddress()) {
            Path = $"/api/texto-japones/revisar/atualizacao/{formattedDate}"
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
        _logger.Information("Enviando {Count} atualizações de Revisar (TextoJapones)...", lista.Count);
        return await _apiClient.PatchAsync("/api/texto-japones/revisar/lista", lista);
    }

    public async Task<bool> DeleteAsync(List<RevisarDto> lista) {
        if (lista == null || !lista.Any()) return true;
        _logger.Information("Deletando {Count} atualizações de Revisar (TextoJapones)...", lista.Count);
        return await _apiClient.DeleteAsync("/api/texto-japones/revisar/lista", lista);
    }
    #endregion

    #region VocabularioDto
    public async Task<PagedApiResponse<VocabularioDto>?> GetUpdatesVocabularioAsync(DateTime lastUpdate, int page = 0) {
        _logger.Information("Buscando atualizações de Vocabulario (TextoJapones), página {Page}...", page);
        string formattedDate = lastUpdate.ToString("yyyy-MM-ddTHH:mm:ss");
        var uriBuilder = new UriBuilder(_apiClient.GetBaseAddress()) {
            Path = $"/api/texto-japones/vocabulario/atualizacao/{formattedDate}"
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
        _logger.Information("Enviando {Count} atualizações de Vocabulario (TextoJapones)...", lista.Count);
        return await _apiClient.PatchAsync("/api/texto-japones/vocabulario/lista", lista);
    }

    public async Task<bool> DeleteAsync(List<VocabularioDto> lista) {
        if (lista == null || !lista.Any()) return true;
        _logger.Information("Deletando {Count} atualizações de Vocabulario (TextoJapones)...", lista.Count);
        return await _apiClient.DeleteAsync("/api/texto-japones/vocabulario/lista", lista);
    }
    #endregion
}
