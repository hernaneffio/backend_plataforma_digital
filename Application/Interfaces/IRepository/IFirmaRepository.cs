using Domain.Entitites.Firma;
using Domain.Payload.Firma;

namespace Application.Interfaces.IRepository;

public interface IFirmaRepository
{
    Task<bool> existeFirmaRepository(CreateFirmaPayload payload);

    Task<bool> existeFirmaRepositoryId(UpdateFirmaPayload payload);

    Task<(string, string)> createFirmaRepository(CreateFirmaPayload payload);

    Task<(string, string)> updateFirmaRepository(UpdateFirmaPayload payload);

    Task<List<FirmaEntity>> listarFirmaRepository();
}
