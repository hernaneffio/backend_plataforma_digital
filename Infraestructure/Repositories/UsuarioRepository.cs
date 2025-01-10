using Application.Interfaces.IRepository;
using Domain.Entitites.Usuario;
using Domain.Payload.Authentication;
using Microsoft.Extensions.Configuration;
using Npgsql;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infraestructure.Repositories;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly string _connectionString;
    //private readonly IHttpContextAccessor _httpContextAccessor;
    public UsuarioRepository(IConfiguration _configuration)
    {
        _connectionString = _configuration.GetConnectionString("PostgresSQLConnection");
        //_httpContextAccessor = httpContextAccessor;
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
}
