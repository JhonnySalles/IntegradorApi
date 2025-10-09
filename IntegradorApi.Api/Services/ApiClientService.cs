using IntegradorApi.Api.Core;
using IntegradorApi.Api.Models;
using Newtonsoft.Json;
using Serilog;
using System.Net.Http.Headers;
using System.Text;

namespace IntegradorApi.Data.Services;

public class ApiClientService {
    public readonly HttpClient _httpClient;
    private readonly Models.Connection _connection;
    private readonly ILogger _logger;

    public ApiClientService(Models.Connection connection, ILogger logger) {
        _connection = connection;
        _logger = logger;
        _httpClient = new HttpClient {
            BaseAddress = new Uri(connection.Address)
        };
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }

    public string GetBaseAddress() {
        return _connection.Address;
    }

    /// <summary>
    /// Tenta fazer login na API usando as credenciais da conexão.
    /// Se bem-sucedido, armazena o token no ApiAuthManager.
    /// </summary>
    /// <returns>Verdadeiro se o login foi bem-sucedido.</returns>
    public async Task<bool> SignInAsync() {
        if (ApiAuthManager.IsAuthenticated)
            return true;

        ApiAuthManager.ClearAuthentication();

        var requestBody = new SignInRequest {
            Username = _connection.User,
            Password = _connection.Password
        };

        var jsonBody = JsonConvert.SerializeObject(requestBody);
        var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

        try {
            var response = await _httpClient.PostAsync("/auth/signin", content);

            if (!response.IsSuccessStatusCode) {
                _logger.Error("Falha no login da API para {Url}. Status: {Status}", _connection.Address, response.StatusCode);
                return false;
            }

            var jsonResponse = await response.Content.ReadAsStringAsync();
            var authResponse = JsonConvert.DeserializeObject<AuthResponse>(jsonResponse);

            if (authResponse?.Authenticated == true && authResponse.AccessToken != null) {
                ApiAuthManager.SetAuthentication(
                    authResponse.AccessToken,
                    authResponse.RefreshToken,
                    authResponse.Username,
                    authResponse.Expiration
                );
                _logger.Information("Autenticado com sucesso na API {Url} como {Username}", _connection.Address, authResponse.Username);
                return true;
            }
        } catch (Exception ex) {
            _logger.Error(ex, "Exceção ao tentar fazer login na API {Url}", _connection.Address);
        }

        return false;
    }

    /// <summary>
    /// Executa uma requisição GET genérica para a API.
    /// </summary>
    /// <typeparam name="T">O tipo do objeto para deserializar a resposta.</typeparam>
    /// <param name="requestUri">A URI do endpoint (ex: "/api/manga-extractor/tables").</param>
    /// <returns>O objeto deserializado ou nulo em caso de erro.</returns>
    public async Task<T?> GetAsync<T>(string requestUri) where T : class {
        if (!await SignInAsync()) {
            _logger.Error("Não foi possível autenticar na API para a requisição GET: {RequestUri}", requestUri);
            return null;
        }

        try {
            // Adiciona o token de autorização no cabeçalho para esta chamada
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", ApiAuthManager.AccessToken);

            var response = await _httpClient.GetAsync(requestUri);

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized) {
                ApiAuthManager.ClearAuthentication();
                _logger.Warning("Token de acesso expirou ou é inválido ao acessar {RequestUri}", requestUri);
                throw new Exception("Token de acesso expirou ou é inválido ao acessar " + requestUri);
            }

            // Lança uma exceção para outros códigos de erro (404, 500, etc.)
            response.EnsureSuccessStatusCode();

            var jsonResponse = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(jsonResponse);
        } catch (Exception ex) {
            _logger.Error(ex, "Exceção na requisição GET para {RequestUri}", requestUri);
            return null;
        }
    }

    /// <summary>
    /// Executa uma requisição PATCH genérica para a API, enviando um objeto como corpo JSON.
    /// </summary>
    /// <param name="requestUri">A URI do endpoint (ex: "/api/manga-extractor/tabela/tabela_teste/lista").</param>
    /// <param name="data">O objeto de dados a ser serializado e enviado.</param>
    /// <returns>Verdadeiro se a operação foi bem-sucedida (status 2xx).</returns>
    public async Task<bool> PatchAsync(string requestUri, object data) {
        if (!await SignInAsync()) {
            _logger.Error("Não foi possível autenticar na API para a requisição PATCH: {RequestUri}", requestUri);
            throw new Exception("Token de acesso expirou ou é inválido ao acessar " + requestUri);
        }

        try {
            var jsonPayload = JsonConvert.SerializeObject(data);
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(HttpMethod.Patch, requestUri) {
                Content = content
            };

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", ApiAuthManager.AccessToken);

            var response = await _httpClient.SendAsync(request);

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized) {
                ApiAuthManager.ClearAuthentication();
                _logger.Warning("Token de acesso expirou ou é inválido ao enviar PATCH para {RequestUri}", requestUri);
                return false;
            }

            if (!response.IsSuccessStatusCode)
                _logger.Error("Requisição PATCH para {RequestUri} falhou com status {StatusCode}. Conteúdo: {ResponseContent}", requestUri, response.StatusCode, await response.Content.ReadAsStringAsync());

            return response.IsSuccessStatusCode;
        } catch (Exception ex) {
            _logger.Error(ex, "Exceção na requisição PATCH para {RequestUri}", requestUri);
            return false;
        }
    }

    /// <summary>
    /// Executa uma requisição DELETE genérica para a API, enviando um objeto como corpo JSON.
    /// </summary>
    /// <param name="requestUri">A URI do endpoint (ex: "/api/manga-extractor/tabela/tabela_teste/lista").</param>
    /// <param name="data">O objeto de dados a ser serializado e enviado.</param>
    /// <returns>Verdadeiro se a operação foi bem-sucedida (status 2xx).</returns>
    public async Task<bool> DeleteAsync(string requestUri, object data) {
        if (!await SignInAsync()) {
            _logger.Error("Não foi possível autenticar na API para a requisição DELETE: {RequestUri}", requestUri);
            throw new Exception("Token de acesso expirou ou é inválido ao acessar " + requestUri);
        }

        try {
            var jsonPayload = JsonConvert.SerializeObject(data);
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(HttpMethod.Delete, requestUri) {
                Content = content
            };

            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", ApiAuthManager.AccessToken);

            var response = await _httpClient.SendAsync(request);

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized) {
                ApiAuthManager.ClearAuthentication();
                _logger.Warning("Token de acesso expirou ou é inválido ao enviar DELETE para {RequestUri}", requestUri);
                return false;
            }

            if (!response.IsSuccessStatusCode)
                _logger.Error("Requisição DELETE para {RequestUri} falhou com status {StatusCode}. Conteúdo: {ResponseContent}", requestUri, response.StatusCode, await response.Content.ReadAsStringAsync());

            return response.IsSuccessStatusCode;
        } catch (Exception ex) {
            _logger.Error(ex, "Exceção na requisição DELETE para {RequestUri}", requestUri);
            return false;
        }
    }
}
