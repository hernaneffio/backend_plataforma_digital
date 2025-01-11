using Application.Interfaces.IRepository;
using Application.Interfaces.IServices;
using Application.Services;
using Infraestructure.Repositories;

namespace Metroli_PDF;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Aquí defines todas las inyecciones de dependencias
        services.AddHttpContextAccessor();

        services.AddTransient<IAuthUserService, AuthUserService>();
        services.AddScoped<IAuthenticationService, AuthenticationService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IUsuarioService, UsuarioService>();
        services.AddScoped<IUsuarioRepository, UsuarioRepository>();

        return services;
    }
}
