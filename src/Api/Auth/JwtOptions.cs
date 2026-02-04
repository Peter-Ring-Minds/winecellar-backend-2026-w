namespace Api.Auth;

public sealed class JwtOptions
{
    public string? Key { get; init; }
    public string Issuer { get; init; } = "Winecellar";
    public string Audience { get; init; } = "Winecellar";
    public int ExpiresMinutes { get; init; } = 60;
}
