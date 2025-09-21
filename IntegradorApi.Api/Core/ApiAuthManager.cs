namespace IntegradorApi.Api.Core;

public static class ApiAuthManager {
    public static string? AccessToken { get; private set; }
    public static string? RefreshToken { get; private set; }
    public static string? Username { get; private set; }
    public static DateTime Expiration { get; private set; }

    public static bool IsAuthenticated => !string.IsNullOrEmpty(AccessToken) && DateTime.UtcNow < Expiration;

    public static void SetAuthentication(string accessToken, string refreshToken, string username, DateTime expiration) {
        AccessToken = accessToken;
        RefreshToken = refreshToken;
        Username = username;
        Expiration = expiration.ToUniversalTime();
    }

    public static void ClearAuthentication() {
        AccessToken = null;
        RefreshToken = null;
        Username = null;
        Expiration = DateTime.MinValue;
    }
}
