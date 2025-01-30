using Domain.Entitites.File;
using Domain.Entitites.Firma;
using Domain.Payload.File;
using Domain.Payload.Firma;

namespace Application.Interfaces.IRepository;

public interface IFileRepository
{
    Task<(string, string)> createFileRepository(CreateFilePayload payload);

    Task<(string, string)> createFileNewRepository(CreateFileNewPayload payload);

    Task<bool> existeFileRepositoryId(int id);

    Task<(bool, string)> deleteFileRepository(DeleteFirmaPayload payload);

    Task<List<FileEntity>> listarFileRepository(string filtro);


}
