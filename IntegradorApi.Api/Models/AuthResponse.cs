using Newtonsoft.Json;

namespace IntegradorApi.Api.Models;

public class AuthResponse {
    [JsonProperty("username")]
    public string? Username { get; set; }

    [JsonProperty("authenticated")]
    public bool Authenticated { get; set; }

    [JsonProperty("created")]
    public DateTime Created { get; set; }

    [JsonProperty("expiration")]
    public DateTime Expiration { get; set; }

    [JsonProperty("accessToken")]
    public string? AccessToken { get; set; }

    [JsonProperty("refreshToken")]
    public string? RefreshToken { get; set; }
}