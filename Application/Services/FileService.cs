using Application.Interfaces.IRepository;
using Application.Interfaces.IServices;
using Domain.Entitites.File;
using Domain.Entitites.Firma;
using Domain.Models;
using Domain.Payload.File;
using Domain.Payload.Firma;
using System.Net;

namespace Application.Services;

public class FileService : IFileService
{

    private readonly IFileRepository _fileRepository;

    public FileService(IFileRepository fileRepository)
    {
        _fileRepository = fileRepository;
    }


    public async Task<MessageResult<string>> createFile(CreateFilePayload payload)
    {
        try
        {
            if (payload.base64File == null || payload.fileName == null)
                throw new ErrorHandler(HttpStatusCode.BadRequest, $"Los datos enviados deben estar completos", null, internalResponse: 2, status: 400);

            var (result, message) = await _fileRepository.createFileRepository(payload);

            if (result == null)
                throw new ErrorHandler(HttpStatusCode.InternalServerError, message, null, internalResponse: 2, status: 500);

            return MessageResult<string>.Of(message, result, (int?)HttpStatusCode.Accepted, 1);
        }
        catch (Exception ex)
        {
            throw new ErrorHandler(HttpStatusCode.InternalServerError, $"Error {ex.InnerException?.Message ?? ex.Message}", null, internalResponse: 2, status: 500);
        }
    }


    public async Task<MessageResult<FileEntityCreate>> createFileNew(CreateFileNewPayload payload)
    {
        try
        {
            if (payload.base64File == null || payload.fileName == null || payload.base64Firma == null)
                throw new ErrorHandler(HttpStatusCode.BadRequest, $"Los datos enviados deben estar completos", null, internalResponse: 2, status: 400);

            var existeFile = await _fileRepository.existeFileRepositoryName(payload.fileName);

            if (existeFile)
                throw new ErrorHandler(HttpStatusCode.BadRequest, $"Ya existe un dato con el nombre", null, internalResponse: 2, status: 400);

            var (result, message) = await _fileRepository.createFileNewRepository(payload);

            if (result == null)
                throw new ErrorHandler(HttpStatusCode.InternalServerError, message, null, internalResponse: 2, status: 500);

            return MessageResult<FileEntityCreate>.Of(message, result, (int?)HttpStatusCode.Accepted, 1);
        }
        catch (Exception ex)
        {
            throw new ErrorHandler(HttpStatusCode.InternalServerError, $"Error {ex.InnerException?.Message ?? ex.Message}", null, internalResponse: 2, status: 500);
        }
    }


    public async Task<MessageResult<string>> updateFile(UpdateFilePayload payload)
    {
        try
        {
            if (payload.base64File == null)
                throw new ErrorHandler(HttpStatusCode.BadRequest, $"Los datos enviados deben estar completos", null, internalResponse: 2, status: 400);

            var existeFile = await _fileRepository.existeFileRepositoryIdName(payload.id);

            if (existeFile == null)
                throw new ErrorHandler(HttpStatusCode.BadRequest, $"El documento a actualizar no existe", null, internalResponse: 2, status: 400);

            var result = await _fileRepository.updateFileRepository(payload, existeFile);

            if (result == null)
                throw new ErrorHandler(HttpStatusCode.BadRequest, "Error al consumir el servicio", null, internalResponse: 2, status: 400);

            return MessageResult<string>.Success(result, "Success", (int?)HttpStatusCode.OK);
        }
        catch(Exception ex)
        {
            throw new ErrorHandler(HttpStatusCode.InternalServerError, $"Error {ex.InnerException?.Message ?? ex.Message}", null, internalResponse: 2, status: 500);
        }
        
        
    }


    public async Task<MessageResult<bool>> deleteFile(DeleteFirmaPayload payload)
    {
        try
        {
            if (payload.id == null)
                throw new ErrorHandler(HttpStatusCode.BadRequest, $"Los datos enviados deben estar completos", null, internalResponse: 2, status: 400);

            var existeFile = await _fileRepository.existeFileRepositoryId(payload.id);

            if (!existeFile)
                throw new ErrorHandler(HttpStatusCode.BadRequest, $"La firma a eliminar no existe", null, internalResponse: 2, status: 400);

            var (result, message) = await _fileRepository.deleteFileRepository(payload);

            if (!result)
                throw new ErrorHandler(HttpStatusCode.InternalServerError, message, null, internalResponse: 2, status: 500);

            return MessageResult<bool>.Of(message, result, (int?)HttpStatusCode.OK, 1);
        }
        catch (Exception ex)
        {
            throw new ErrorHandler(HttpStatusCode.InternalServerError, $"Error {ex.InnerException?.Message ?? ex.Message}", null, internalResponse: 2, status: 500);
        }
    }


    public async Task<MessageResult<List<FileEntity>>> listarFile(string filtro)
    {

        try
        {

            var result = await _fileRepository.listarFileRepository(filtro);

            return MessageResult<List<FileEntity>>.Of("Succeeded", result, (int?)HttpStatusCode.OK, 1);

        }
        catch (Exception ex)
        {
            throw new ErrorHandler(HttpStatusCode.BadRequest, $"Error {ex.InnerException?.Message ?? ex.Message}", null, internalResponse: 2, status: 400);
        }


    }
}
