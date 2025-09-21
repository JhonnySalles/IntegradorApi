using Newtonsoft.Json;

namespace IntegradorApi.Api.Models;

public class SignInRequest {
    [JsonProperty("username")]
    public string Username { get; set; }

    [JsonProperty("password")]
    public string Password { get; set; }
}