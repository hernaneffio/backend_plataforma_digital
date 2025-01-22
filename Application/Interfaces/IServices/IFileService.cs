using Domain.Models;
using Domain.Payload.File;

namespace Application.Interfaces.IServices;

public interface IFileService
{
    Task<MessageResult<string>> createFile(UpdateFilePayload payload);

    //Task<MessageResult<string>> updateFile(UpdateFilePayload payload);

    //Task<MessageResult<string>> listarFile(UpdateFilePayload payload);
}
