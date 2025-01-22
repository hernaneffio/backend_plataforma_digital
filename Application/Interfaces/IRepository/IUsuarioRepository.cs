using Domain.Entitites.Usuario;
using Domain.Payload.Authentication;

namespace Application.Interfaces.IRepository;

public interface IUsuarioRepository
{
    Task<UsuarioEntity> CheckUsuario(LoginPayload payload);

    Task<int> CheckUsuarioToken();

    Task<UsuarioMeEntity> GetUsuarioMe(int userId);
}
