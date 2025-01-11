using Application.Interfaces.IServices;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services;

public class AuthUserService( IHttpContextAccessor httpContextAccessor) : IAuthUserService
{
    public int GetUserId()
    {
        var context = httpContextAccessor.HttpContext;
        var userId = context?.User.FindFirst("userId");
        return int.Parse(userId?.Value ?? "0");
    }
}
