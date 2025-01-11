using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Application.Interfaces.IServices;

namespace Metroli_PDF.Middleware;

public class ValidacionTokenPersonalizadaMiddleware
{
    private readonly RequestDelegate _next;

    public ValidacionTokenPersonalizadaMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(
        HttpContext context,
        IAuthenticationService _authenticationService)
    {
        var endpoint = context.GetEndpoint();
        var anonymous = endpoint?.Metadata.GetMetadata<IAllowAnonymous>();
        if (anonymous != null)
        {
            // Si [AllowAnonymous] está presente, simplemente continúa con la siguiente middleware en el pipeline
            await _next(context);
            return;
        }
        var token = context.Request.Headers["Authorization"].FirstOrDefault();
        var tokenSplited = (token ?? "").Split(" ");
        if (tokenSplited != null && tokenSplited.Length > 1)
        {
            var tokenPart = tokenSplited[1];
            token = tokenPart ?? "";
        }

        var (result, userId) = await _authenticationService.ValidateToken(token ?? "");
        if (result != null)
        {
            // autorizacion correcta
            // setea en los claims el id del usuario
            context.User.AddIdentity(new ClaimsIdentity([
                new Claim("userId", userId ?? "")
            ]));
            await _next(context);
            return;
        }
        else
        {
            //var errors = new List<ValidationError>
            //{
            //    new () {
            //        Index = 1,
            //        Error = "El token no es válido"
            //    }
            //};

            //var errorResult = MessageValidatedResult<object>.Fault(2, "Error de Validaciones", errors, 401);

            // error del token
            context.Response.StatusCode = 401; // Unauthorized
            context.Response.ContentType = "application/json";
            //await context.Response.WriteAsync(JsonConvert.SerializeObject(errorResult));
            return;
        }
    }
}
