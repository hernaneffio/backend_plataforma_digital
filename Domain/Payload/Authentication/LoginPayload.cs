using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Payload.Authentication;

public class LoginPayload
{
    public string username { get; set; }

    public string password { get; set; }
}
