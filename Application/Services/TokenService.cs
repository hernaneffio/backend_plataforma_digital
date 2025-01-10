using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Application.Interfaces.IServices;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;

namespace Application.Services;

public class TokenService : ITokenService
{
    private readonly IConfiguration _configuration;
    public TokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }


    public async Task<string> generateUserToken(
        int usuarioId,
        DateTime fechaExpiracion,
        string type)
    {
        var handler = new JwtSecurityTokenHandler();
        // Usa una clave de al menos 32 caracteres (256 bits)
        // generar con: openssl rand -hex 32
        var key = Encoding.ASCII.GetBytes(
            _configuration["TokenManagement:JwtSecretKey"]
        );

        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(key),
            SecurityAlgorithms.HmacSha256Signature);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = GenerateClaims(usuarioId, type),
            Expires = fechaExpiracion,
            SigningCredentials = credentials,
        };

        var token = handler.CreateToken(tokenDescriptor);
        var generatedToken = handler.WriteToken(token);
        return generatedToken ?? "";
    }





    public async Task<IDictionary<string, object>?> ValidateToken(
        string token)
    {
        var handler = new JwtSecurityTokenHandler();

        var key = Encoding.ASCII.GetBytes(
            _configuration["TokenManagement:JwtSecretKey"]
        );

        var parameters = new TokenValidationParameters()
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false,
            ValidateAudience = false,
            RequireExpirationTime = false,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero,
        };

        var result = await handler.ValidateTokenAsync(
            token: token,
            validationParameters: parameters);

        if (!result.IsValid)
        {
            return null;
        }

        return result.Claims;
    }


    public async Task<string> validateRefleshToken(string refleshtoken)
    {

        var handler = new JwtSecurityTokenHandler();
        var jsonToken = handler.ReadJwtToken(refleshtoken);


        var sub = jsonToken.Payload["sub"];

        string result = sub.ToString();

        return result;
    }

    private static ClaimsIdentity GenerateClaims(int usuarioId, string type)
    {
        var claims = new ClaimsIdentity();
        claims.AddClaim(new Claim("sub", usuarioId.ToString()));
        claims.AddClaim(new Claim("type", type));

        return claims;
    }
}

