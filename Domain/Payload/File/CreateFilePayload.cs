namespace Domain.Payload.File;

public class CreateFilePayload
{
    public string base64File { get; set; }

    public string fileName { get; set; }

    public List<FirmasList> firmas { get; set; }

}

public class FirmasList
{
    public string ruta { get; set; }

}
