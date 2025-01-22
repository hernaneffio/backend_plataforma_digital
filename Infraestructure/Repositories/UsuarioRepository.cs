using Application.Interfaces.IRepository;
using Dapper;
using Domain.Entitites.Usuario;
using Domain.Payload.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace Infraestructure.Repositories;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly string _connectionString;
    private readonly IHttpContextAccessor _httpContextAccessor;
    public UsuarioRepository(IConfiguration _configuration, IHttpContextAccessor httpContextAccessor)
    {
        _connectionString = _configuration.GetConnectionString("PostgresSQLConnection");
        ////_httpContextAccessor = httpContextAccessor;
    }

    public async Task<UsuarioEntity> CheckUsuario(LoginPayload payload)
    {
        try
        {
            var result1 = new UsuarioEntity();

            using (var connection = new NpgsqlConnection(_connectionString))
            //using (var connection = new NpgsqlConnection(_connectionString))
            {
                string query = @"
                        SELECT 
                            u_id as id,
                            u_password as password
                        FROM metroli.mst_usuarios
                        WHERE u_user = @username
                    ";

                var parameters = new
                {
                    username = payload.username
                };

                result1 = await connection.QueryFirstOrDefaultAsync<UsuarioEntity>(query, parameters);

                //result = connection.Query<TiendaEntity>(query).ToList();
            }

            return result1;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(ex.InnerException?.Message ?? ex.Message);
        }
    }


    public async Task<int> CheckUsuarioToken()
    {
        var context = _httpContextAccessor.HttpContext;
        var userId = context?.User.FindFirst("userId");

        return int.Parse(userId?.Value ?? "0");
    }


    public async Task<UsuarioMeEntity> GetUsuarioMe(int userId)
    {
        try
        {
            var result = new UsuarioMeEntity();

            using (var connection = new NpgsqlConnection(_connectionString))
            //using (var connection = new NpgsqlConnection(_connectionString))
            {
                string query = @"
                        SELECT 
                            u.u_id as id,
                            u.u_nombres as nombre,
                            u.u_apellidos as apellido,
                            u.u_user as user,
                            u.u_estado as estado
                        FROM metroli.mst_usuarios u
                        WHERE u.u_id = @id
                    ";

                var parameters = new
                {
                    id = userId
                };

                result = await connection.QueryFirstOrDefaultAsync<UsuarioMeEntity>(query, parameters);

                //result = connection.Query<TiendaEntity>(query).ToList();
            }

            return result;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(ex.InnerException?.Message ?? ex.Message);
        }
    }
}
