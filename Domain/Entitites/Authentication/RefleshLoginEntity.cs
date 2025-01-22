namespace Domain.Entitites.Authentication;

public class RefleshLoginEntity
{
    public string token { get; set; }

    public string tokenExpirationAt { get; set; }

    public string refreshToken { get; set; }

    public string refreshTokenExpiration { get; set; }
}
