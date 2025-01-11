using Application.Interfaces.IRepository;
using Application.Interfaces.IServices;
using Domain.Entitites.Usuario;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services;

public class UsuarioService : IUsuarioService
{
    
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IAuthUserService _authUserService;

    public UsuarioService(IUsuarioRepository usuarioRepository, IAuthUserService authUserService)
    {
        _usuarioRepository = usuarioRepository;
        _authUserService = authUserService;
    }

    public async Task<MessageResult<UsuarioMeEntity>> getMe()
    {
        try
        {

            int idUsuario = _authUserService.GetUserId();

            var usuarioMe = await _usuarioRepository.GetUsuarioMe(idUsuario);

            if (usuarioMe == null)
                throw new InvalidOperationException("Usuario no encontrado");

            return MessageResult<UsuarioMeEntity>.Of("Succeeded", usuarioMe, (int?)HttpStatusCode.Accepted, 1);
        }
        catch (Exception ex)
        {
            throw new ErrorHandler(HttpStatusCode.BadRequest, $"Error {ex.InnerException?.Message ?? ex.Message}", null, internalResponse: 2, status: 400);
        }

    }
}
