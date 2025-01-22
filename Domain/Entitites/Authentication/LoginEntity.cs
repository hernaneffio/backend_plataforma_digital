namespace Domain.Entitites.Authentication;

public class LoginEntity
{
    public bool isAuthenticated { get; set; } = false;

    public string username { get; set; }

    public string token { get; set; }

    public string tokenExpirationAt { get; set; }

    public string refreshToken { get; set; }

    public string refreshTokenExpiration { get; set; }
}
