using Application.Interfaces.IRepository;
using Infraestructure.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Org.BouncyCastle.Security;

namespace Infraestructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpContextAccessor(); // Registra IHttpContextAccessor en la capa de infraestructura
        services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        return services;
    }
}
