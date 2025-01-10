using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entitites.Authentication;
using Domain.Models;
using Domain.Payload.Authentication;

namespace Application.Interfaces.IServices;

public interface IAuthenticationService
{
    Task<MessageResult<LoginEntity>> Login(LoginPayload payload);

    Task<RefleshLoginEntity> refleshLogin(RefleshTokenPayload payload);

    Task<(IDictionary<string, object>?, string?)> ValidateToken(string token);
}
