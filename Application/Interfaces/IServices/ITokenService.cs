using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
