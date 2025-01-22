namespace Domain.Payload.File;

public class UpdateFilePayload
{
    public string base64File { get; set; }

    public string fileName { get; set; }
}
