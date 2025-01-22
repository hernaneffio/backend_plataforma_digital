using Application.Interfaces.IRepository;
using Application.Interfaces.IServices;
using Domain.Models;
using Domain.Payload.File;
using System.Net;

namespace Application.Services;

public class FileService : IFileService
{

    private readonly IFileRepository _fileRepository;

    public FileService(IFileRepository fileRepository)
    {
        _fileRepository = fileRepository;
    }


    public async Task<MessageResult<string>> createFile(UpdateFilePayload payload)
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
}
