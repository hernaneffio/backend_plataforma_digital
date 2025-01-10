using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entitites.Authentication;

public class LoginEntity
{
    public bool isAuthenticated { get; set; } = false;

    public string username { get; set; }

    public string token { get; set; }

    public string tokenExpirationAt { get; set; }

    public string refreshToken { get; set; }

    public string refreshTokenExpiration { get; set; }
}
