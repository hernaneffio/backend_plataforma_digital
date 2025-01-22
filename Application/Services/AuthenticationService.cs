using Application.Interfaces.IRepository;
using Application.Interfaces.IServices;
using Domain.Entitites.Authentication;
using Domain.Models;
using Domain.Payload.Authentication;
using Microsoft.Extensions.Configuration;
using System.Net;

namespace Application.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly IConfiguration _configuration;
    private readonly ITokenService _TokenService;
    private readonly IUsuarioRepository _usuarioRepository;

    public AuthenticationService(IConfiguration configuration, ITokenService tokenService, IUsuarioRepository usuarioRepository)
    {
        _configuration = configuration;
        _TokenService = tokenService;
        _usuarioRepository = usuarioRepository;
    }

    public async Task<MessageResult<LoginEntity>> Login(LoginPayload payload)
    {
        try
        {


            var result = await _usuarioRepository.CheckUsuario(payload);

            if (result == null)
                throw new InvalidOperationException("Ingrese un usuario válido");
            //throw new ErrorHandler(HttpStatusCode.UnprocessableEntity, "Ingrese un usuario válido", null, status: 422);

            var isValidPassword = BCrypt.Net.BCrypt
                        .Verify(payload.password, result.password ?? "");

            if (!isValidPassword)
                throw new InvalidOperationException("Ingrese una contraseña correcta");
            //throw new ErrorHandler(HttpStatusCode.UnprocessableEntity, "Ingrese una contraseña correcta", null, status: 422);

            var limaTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SA Pacific Standard Time");
            var limaDateTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, limaTimeZone);


            var tokenExpirationAt = limaDateTime.AddMinutes(
                        int.Parse(_configuration["TokenManagement:JwtAccessTokenExpiration"])
                    );
            var token = await _TokenService.generateUserToken(
                usuarioId: result?.id ?? 0,
                fechaExpiracion: tokenExpirationAt,
                type: "token"
            );

            var refreshTokenExpirationAt = limaDateTime.AddMinutes(
                int.Parse(_configuration["TokenManagement:JwtRefreshTokenExpiration"])
            );
            var refreshToken = await _TokenService.generateUserToken(
                usuarioId: result?.id ?? 0,
                fechaExpiracion: refreshTokenExpirationAt,
                type: "refresh"
            );

            var model = new LoginEntity()
            {
                isAuthenticated = true,
                username = payload.username,
                token = token,
                tokenExpirationAt = tokenExpirationAt.ToString("dd/MM/yyyy HH:mm:ss"),
                refreshToken = refreshToken,
                refreshTokenExpiration = refreshTokenExpirationAt.ToString("dd/MM/yyyy HH:mm:ss")
            };


            //var result = new LoginEntity();


            return MessageResult<LoginEntity>.Of("Succeeded", model, (int?)HttpStatusCode.Accepted, 1);

        }
        catch (Exception ex)
        {
            throw new ErrorHandler(HttpStatusCode.BadRequest, $"Error : {ex.InnerException?.Message ?? ex.Message}", null, internalResponse: 2, status: 400);
        }
    }

    public async Task<RefleshLoginEntity> refleshLogin(RefleshTokenPayload payload)
    {

        var id = await _TokenService.validateRefleshToken(payload.refleshToken);

        //if(id != "15")
        //    throw new InvalidOperationException("Token incorrecto");


        var limaTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SA Pacific Standard Time");
        var limaDateTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, limaTimeZone);

        var tokenExpirationAt = limaDateTime.AddMinutes(
                    int.Parse(_configuration["TokenManagement:JwtAccessTokenExpiration"])
                );
        var token = await _TokenService.generateUserToken(
            usuarioId: int.Parse(id),
            fechaExpiracion: tokenExpirationAt,
            type: "token"
        );

        var refreshTokenExpirationAt = limaDateTime.AddMinutes(
            int.Parse(_configuration["TokenManagement:JwtRefreshTokenExpiration"])
        );
        var refreshToken = await _TokenService.generateUserToken(
            usuarioId: int.Parse(id),
            fechaExpiracion: refreshTokenExpirationAt,
            type: "refresh"
        );


        var result = new RefleshLoginEntity()
        {
            token = token,
            tokenExpirationAt = tokenExpirationAt.ToString("dd/MM/yyyy HH:mm:ss"),
            refreshToken = refreshToken,
            refreshTokenExpiration = refreshTokenExpirationAt.ToString("dd/MM/yyyy HH:mm:ss")
        };


        return result;
    }



    public async Task<(IDictionary<string, object>?, string?)> ValidateToken(string token)
    {
        var result = await _TokenService.ValidateToken(token);
        string? userId;
        if (result == null)
        {
            return (null, null);
        }

        // Verifica que el valor "sub" existe y no es nulo. 
        if (
            result.TryGetValue(
                "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier", out var subObj
            ) && subObj is string sub
        )
        {
            string id = sub;
            userId = id;
        }
        else
        {
            // Maneja el caso en que "sub" no está presente o es nulo.
            return (null, null);
        }

        return (result, userId);
    }
}
