using Domain.Entitites.Usuario;
using Domain.Models;

namespace Application.Interfaces.IServices;

public interface IUsuarioService
{
    Task<MessageResult<UsuarioMeEntity>> getMe();
}
