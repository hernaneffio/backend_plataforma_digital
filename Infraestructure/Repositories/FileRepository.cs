using Amazon;
using Amazon.S3;
using Amazon.S3.Transfer;
using Application.Interfaces.IRepository;
using Domain.Payload.File;
using Microsoft.Extensions.Configuration;

namespace Infraestructure.Repositories;

public class FileRepository : IFileRepository
{

    private readonly IAmazonS3 _s3Client;
    private readonly string _bucketName;

    public FileRepository(IConfiguration configuration)
    {
        var awsOptions = configuration.GetSection("AWS");
        _bucketName = awsOptions["BucketName"];

        _s3Client = new AmazonS3Client(
            awsOptions["AccessKey"],
            awsOptions["SecretKey"],
            RegionEndpoint.GetBySystemName(awsOptions["Region"])
        );
    }


    public async Task<(string, string)> createFileRepository(UpdateFilePayload payload)
    {

        try
        {
            var fileBytes = Convert.FromBase64String(payload.base64File);

            // Crear un stream desde los bytes
            using var memoryStream = new MemoryStream(fileBytes);

            var filename = payload.fileName
                            .Replace(" ", "-") // Reemplaza espacios con guiones
                            .Trim(); // Elimina posibles espacios en los extremos

            // Ruta donde se guardará el archivo en S3
            var key = $"pruebas/{filename}.pdf";

            // Configurar la solicitud de carga
            var request = new TransferUtilityUploadRequest
            {
                InputStream = memoryStream,
                Key = key,
                BucketName = _bucketName,
                ContentType = "application/pdf"
            };

            // Subir el archivo a S3
            var transferUtility = new TransferUtility(_s3Client);
            await transferUtility.UploadAsync(request);

            // Devolver la URL del archivo guardado
            string fileUrl = $"https://{_bucketName}.s3.{RegionEndpoint.GetBySystemName("us-east-2").SystemName}.amazonaws.com/{key}";

            return (fileUrl, "Succeeded");

        }
        catch (Exception ex)
        {

            return (null, $"Error : {ex.InnerException?.Message ?? ex.Message}");
        }

    }
}
