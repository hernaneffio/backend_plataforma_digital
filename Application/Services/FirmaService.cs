using Application.Interfaces.IRepository;
using Application.Interfaces.IServices;
using Domain.Entitites.Firma;
using Domain.Models;
using Domain.Payload.Firma;
using System.Net;

namespace Application.Services;

public class FirmaService : IFirmaService
{
    private readonly IFirmaRepository _firmaRepository;

    public FirmaService(IFirmaRepository firmaRepository)
    {
        _firmaRepository = firmaRepository;
    }

    public async Task<MessageResult<string>> createFirma(CreateFirmaPayload payload)
    {

        try
        {
            if (payload.base64File == null || payload.fileName == null)
                throw new ErrorHandler(HttpStatusCode.BadRequest, $"Los datos enviados deben estar completos", null, internalResponse: 2, status: 400);

            var existeFirma = await _firmaRepository.existeFirmaRepository(payload);

            if (existeFirma)
                throw new ErrorHandler(HttpStatusCode.BadRequest, $"El nombre de esa firma ya existe", null, internalResponse: 2, status: 400);

            var (result, message) = await _firmaRepository.createFirmaRepository(payload);

            if (result == null)
                throw new ErrorHandler(HttpStatusCode.InternalServerError, message, null, internalResponse: 2, status: 500);

            return MessageResult<string>.Of(message, result, (int?)HttpStatusCode.OK, 1);
        }
        catch (Exception ex)
        {
            throw new ErrorHandler(HttpStatusCode.InternalServerError, $"Error {ex.InnerException?.Message ?? ex.Message}", null, internalResponse: 2, status: 500);
        }

    }

    public async Task<MessageResult<string>> updateFirma(UpdateFirmaPayload payload)
    {
        try
        {
            if (payload.base64File == null || payload.fileName == null)
                throw new ErrorHandler(HttpStatusCode.BadRequest, $"Los datos enviados deben estar completos", null, internalResponse: 2, status: 400);

            var existeFirma = await _firmaRepository.existeFirmaRepositoryId(payload.id);

            if (!existeFirma)
                throw new ErrorHandler(HttpStatusCode.BadRequest, $"La firma a actualizar no existe", null, internalResponse: 2, status: 400);

            var (result, message) = await _firmaRepository.updateFirmaRepository(payload);

            if (result == null)
                throw new ErrorHandler(HttpStatusCode.InternalServerError, message, null, internalResponse: 2, status: 500);

            return MessageResult<string>.Of(message, result, (int?)HttpStatusCode.OK, 1);
        }
        catch (Exception ex)
        {
            throw new ErrorHandler(HttpStatusCode.InternalServerError, $"Error {ex.InnerException?.Message ?? ex.Message}", null, internalResponse: 2, status: 500);
        }
    }

    public async Task<MessageResult<List<FirmaEntity>>> listarFirma(string filtro)
    {

        try
        {

            var result = await _firmaRepository.listarFirmaRepository(filtro);

            return MessageResult<List<FirmaEntity>>.Of("Succeeded", result, (int?)HttpStatusCode.OK, 1);

        }
        catch (Exception ex)
        {
            throw new ErrorHandler(HttpStatusCode.BadRequest, $"Error {ex.InnerException?.Message ?? ex.Message}", null, internalResponse: 2, status: 400);
        }


    }

    public async Task<MessageResult<bool>> deleteFirma(DeleteFirmaPayload payload)
    {
        try
        {
            if (payload.id == null)
                throw new ErrorHandler(HttpStatusCode.BadRequest, $"Los datos enviados deben estar completos", null, internalResponse: 2, status: 400);

            var existeFirma = await _firmaRepository.existeFirmaRepositoryId(payload.id);

            if (!existeFirma)
                throw new ErrorHandler(HttpStatusCode.BadRequest, $"La firma a eliminar no existe", null, internalResponse: 2, status: 400);

            var (result, message) = await _firmaRepository.deleteFirmaRepository(payload);

            if (!result)
                throw new ErrorHandler(HttpStatusCode.InternalServerError, message, null, internalResponse: 2, status: 500);

            return MessageResult<bool>.Of(message, result, (int?)HttpStatusCode.OK, 1);
        }
        catch (Exception ex)
        {
            throw new ErrorHandler(HttpStatusCode.InternalServerError, $"Error {ex.InnerException?.Message ?? ex.Message}", null, internalResponse: 2, status: 500);
        }
    }
}
