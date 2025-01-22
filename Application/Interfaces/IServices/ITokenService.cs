namespace Application.Interfaces.IServices;

public interface ITokenService
{
    Task<string> generateUserToken(
        int usuarioId,
        DateTime fechaExpiracion,
        string type);

    Task<IDictionary<string, object>?> ValidateToken(
        string token);

    Task<string> validateRefleshToken(string refleshtoken);
}
