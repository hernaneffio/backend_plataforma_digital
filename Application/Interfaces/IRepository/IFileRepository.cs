using Domain.Payload.File;

namespace Application.Interfaces.IRepository;

public interface IFileRepository
{
    Task<(string, string)> createFileRepository(UpdateFilePayload payload);


}
