namespace Domain.Payload.Firma;

public class UpdateFirmaPayload
{
    public int id { get; set; }

    public string base64File { get; set; }

    public string fileName { get; set; }
}
