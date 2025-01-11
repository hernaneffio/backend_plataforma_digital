using Domain.Entitites.Usuario;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces.IServices;

public interface IUsuarioService
{
    Task<MessageResult<UsuarioMeEntity>> getMe();
}
