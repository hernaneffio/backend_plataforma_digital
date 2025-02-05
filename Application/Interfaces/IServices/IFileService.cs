using Domain.Entitites.File;
using Domain.Entitites.Firma;
using Domain.Models;
using Domain.Payload.File;
using Domain.Payload.Firma;

namespace Application.Interfaces.IServices;

public interface IFileService
{
    Task<MessageResult<string>> createFile(CreateFilePayload payload);

    Task<MessageResult<FileEntityCreate>> createFileNew(CreateFileNewPayload payload);

    Task<MessageResult<string>> updateFile(UpdateFilePayload payload);

    Task<MessageResult<bool>> deleteFile(DeleteFirmaPayload payload);

    Task<MessageResult<List<FileEntity>>> listarFile(string filtro);

}
