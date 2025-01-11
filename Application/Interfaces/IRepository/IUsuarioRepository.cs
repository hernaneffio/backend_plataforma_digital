using Domain.Payload.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entitites.Usuario;

namespace Application.Interfaces.IRepository;

public interface IUsuarioRepository
{
    Task<UsuarioEntity> CheckUsuario(LoginPayload payload);

    Task<int> CheckUsuarioToken();

    Task<UsuarioMeEntity> GetUsuarioMe(int userId);
}
