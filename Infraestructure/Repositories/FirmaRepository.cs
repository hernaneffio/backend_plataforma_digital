using Amazon;
using Amazon.S3;
using Amazon.S3.Transfer;
using Application.Interfaces.IRepository;
using Dapper;
using Domain.Entitites.Firma;
using Domain.Payload.File;
using Domain.Payload.Firma;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace Infraestructure.Repositories;

public class FirmaRepository : IFirmaRepository
{
    private readonly IAmazonS3 _s3Client;
    private readonly string _bucketName;
    private readonly string _connectionString;

    public FirmaRepository(IConfiguration configuration, IConfiguration _configuration)
    {
        var awsOptions = configuration.GetSection("AWS");
        _bucketName = awsOptions["BucketName"];

        _s3Client = new AmazonS3Client(
            awsOptions["AccessKey"],
            awsOptions["SecretKey"],
            RegionEndpoint.GetBySystemName(awsOptions["Region"])
        );
        _connectionString = _configuration.GetConnectionString("PostgresSQLConnection");
    }

    public async Task<bool> existeFirmaRepository(CreateFirmaPayload payload)
    {
        try
        {

            using (var connection = new NpgsqlConnection(_connectionString))
            //using (var connection = new NpgsqlConnection(_connectionString))
            {

                connection.Open();

                string query = @" SELECT count(*)
                                     FROM metroli.mst_firmas 
                                        WHERE f_descripcion=@valor and f_estado=true;";


                var parameters = new
                {
                    valor = payload.fileName
                };

                var result = await connection.QueryFirstOrDefaultAsync<int>(query, parameters);

                if (result > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }

        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(ex.InnerException?.Message ?? ex.Message);
        }
    }

    public async Task<bool> existeFirmaRepositoryId(UpdateFirmaPayload payload)
    {
        try
        {

            using (var connection = new NpgsqlConnection(_connectionString))
            //using (var connection = new NpgsqlConnection(_connectionString))
            {

                connection.Open();

                string query = @" SELECT count(*)
                                     FROM metroli.mst_firmas 
                                        WHERE f_id=@valor and f_estado=true;";


                var parameters = new
                {
                    valor = payload.id
                };

                var result = await connection.QueryFirstOrDefaultAsync<int>(query, parameters);

                if (result > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }

        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(ex.InnerException?.Message ?? ex.Message);
        }
    }

    public async Task<List<FirmaEntity>> listarFirmaRepository()
    {
        try
        {
            var result = new List<FirmaEntity>();

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                string query = @"
                        SELECT 
                            f_id as id,
                            f_descripcion as fileName,
                            f_ruta as fileRuta,
                            f_estado as estado
                        FROM metroli.mst_firmas
                        WHERE f_estado=true
                    ";

                result = connection.Query<FirmaEntity>(query).ToList();

            }

            return result;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(ex.InnerException?.Message ?? ex.Message);
        }
    }

    public async Task<(string, string)> createFirmaRepository(CreateFirmaPayload payload)
    {

        try
        {
            var formatResult = string.Empty;
            var formatExtension = string.Empty;

            var formato = payload.base64File.Substring(0, 20);

            if (formato.Contains("jpeg"))
            {
                formatResult = "data:image/jpeg;base64,";
                formatExtension = "jpeg";
            }
            else if (formato.Contains("jpg"))
            {
                formatResult = "data:image/jpg;base64,";
                formatExtension = "jpg";
            }
            else
            {
                formatResult = "data:image/png;base64,";
                formatExtension = "png";
            }



            var fileBytes = Convert.FromBase64String(payload.base64File.Replace(formatResult, string.Empty));

            // Crear un stream desde los bytes
            using var memoryStream = new MemoryStream(fileBytes);

            var filename = payload.fileName
                            .Replace(" ", "-") // Reemplaza espacios con guiones
                            .Trim(); // Elimina posibles espacios en los extremos

            // Ruta donde se guardará el archivo en S3
            var key = $"firmas/{filename}.{formatExtension}";

            // Configurar la solicitud de carga
            var request = new TransferUtilityUploadRequest
            {
                InputStream = memoryStream,
                Key = key,
                BucketName = _bucketName,
                ContentType = $"image/{formatExtension}"
            };

            // Subir el archivo a S3
            var transferUtility = new TransferUtility(_s3Client);
            await transferUtility.UploadAsync(request);

            // Devolver la URL del archivo guardado
            string fileUrl = $"https://{_bucketName}.s3.{RegionEndpoint.GetBySystemName("us-east-2").SystemName}.amazonaws.com/{key}";

            await createFirmaRepositoryBD(payload.fileName, fileUrl);

            return (fileUrl, "Succeeded");

        }
        catch (Exception ex)
        {
            return (null, $"Error : {ex.InnerException?.Message ?? ex.Message}");
        }

    }


    public async Task<(string, string)> updateFirmaRepository(UpdateFirmaPayload payload)
    {
        try
        {
            var formatResult = string.Empty;
            var formatExtension = string.Empty;

            var formato = payload.base64File.Substring(0, 20);

            if (formato.Contains("jpeg"))
            {
                formatResult = "data:image/jpeg;base64,";
                formatExtension = "jpeg";
            }
            else if (formato.Contains("jpg"))
            {
                formatResult = "data:image/jpg;base64,";
                formatExtension = "jpg";
            }
            else
            {
                formatResult = "data:image/png;base64,";
                formatExtension = "png";
            }



            var fileBytes = Convert.FromBase64String(payload.base64File.Replace(formatResult, string.Empty));

            // Crear un stream desde los bytes
            using var memoryStream = new MemoryStream(fileBytes);

            var filename = payload.fileName
                            .Replace(" ", "-") // Reemplaza espacios con guiones
                            .Trim(); // Elimina posibles espacios en los extremos

            // Ruta donde se guardará el archivo en S3
            var key = $"firmas/{filename}.{formatExtension}";

            // Configurar la solicitud de carga
            var request = new TransferUtilityUploadRequest
            {
                InputStream = memoryStream,
                Key = key,
                BucketName = _bucketName,
                ContentType = $"image/{formatExtension}"
            };

            // Subir el archivo a S3
            var transferUtility = new TransferUtility(_s3Client);
            await transferUtility.UploadAsync(request);

            // Devolver la URL del archivo guardado
            string fileUrl = $"https://{_bucketName}.s3.{RegionEndpoint.GetBySystemName("us-east-2").SystemName}.amazonaws.com/{key}";

            await updateFirmaRepositoryBD(payload, fileUrl);

            return (fileUrl, "Succeeded");

        }
        catch (Exception ex)
        {
            return (null, $"Error : {ex.InnerException?.Message ?? ex.Message}");
        }
    }




    public async Task<string> listarFirmaRepository(UpdateFilePayload payload)
    {
        return "message";
    }


    private async Task createFirmaRepositoryBD(string fileName, string rutaFile)
    {
        try
        {

            using (var connection = new NpgsqlConnection(_connectionString))
            //using (var connection = new NpgsqlConnection(_connectionString))
            {

                connection.Open();

                string query = @" INSERT INTO metroli.mst_firmas
                                       (f_descripcion,
                                        f_ruta,
                                        f_estado)
                                     VALUES(
                                        @descripcion,
                                        @ruta,
                                        @estado); 
                                        SELECT LASTVAL();";


                var parameters = new
                {
                    descripcion = fileName,
                    ruta = rutaFile,
                    estado = true
                };

                var result = await connection.QueryFirstOrDefaultAsync<int>(query, parameters);

            }

        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(ex.InnerException?.Message ?? ex.Message);
        }
    }


    private async Task updateFirmaRepositoryBD(UpdateFirmaPayload payload, string rutaFile)
    {
        try
        {

            using (var connection = new NpgsqlConnection(_connectionString))
            //using (var connection = new NpgsqlConnection(_connectionString))
            {

                connection.Open();

                string query = @" UPDATE metroli.mst_firmas
                                  SET f_descripcion = @descripcion,
                                      f_ruta = @ruta
                                     WHERE f_id = @id and f_estado = TRUE;";


                var parameters = new
                {
                    descripcion = payload.fileName,
                    ruta = rutaFile,
                    id = payload.id
                };

                var result = await connection.QueryFirstOrDefaultAsync<int>(query, parameters);

            }

        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(ex.InnerException?.Message ?? ex.Message);
        }
    }
}