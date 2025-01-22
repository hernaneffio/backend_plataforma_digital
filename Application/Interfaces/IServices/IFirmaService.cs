using Domain.Entitites.Firma;
using Domain.Models;
using Domain.Payload.Firma;

namespace Application.Interfaces.IServices;

public interface IFirmaService
{
    Task<MessageResult<string>> createFirma(CreateFirmaPayload payload);

    Task<MessageResult<string>> updateFirma(UpdateFirmaPayload payload);

    Task<MessageResult<List<FirmaEntity>>> listarFirma();
}
