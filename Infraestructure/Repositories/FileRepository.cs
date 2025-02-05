using Amazon;
using Amazon.S3;
using Amazon.S3.Transfer;
using Application.Interfaces.IRepository;
using Domain.Payload.File;
using Microsoft.Extensions.Configuration;
using Npgsql;
using Dapper;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.IO.Image;
using static System.Net.Mime.MediaTypeNames;
using iText.Barcodes;
using iText.Kernel.Colors;
using iText.Kernel.Pdf.Canvas;
using System.Xml.Linq;
using Domain.Payload.Firma;
using Domain.Entitites.Firma;
using Domain.Entitites.File;

namespace Infraestructure.Repositories;

public class FileRepository : IFileRepository
{

    private readonly IAmazonS3 _s3Client;
    private readonly string _bucketName;
    private readonly string _connectionString;

    public FileRepository(IConfiguration configuration)
    {
        var awsOptions = configuration.GetSection("AWS");
        _bucketName = awsOptions["BucketName"];

        _s3Client = new AmazonS3Client(
            awsOptions["AccessKey"],
            awsOptions["SecretKey"],
            RegionEndpoint.GetBySystemName(awsOptions["Region"])
        );
        _connectionString = configuration.GetConnectionString("PostgresSQLConnection");

    }


    public async Task<(string, string)> createFileRepository(CreateFilePayload payload)
    {

        try
        {

            string fileFirmasPdf = await insertFirma(payload.base64File, payload.firmas);

            var filename = payload.fileName
                            .Replace(" ", "-") // Reemplaza espacios con guiones
                            .Trim(); // Elimina posibles espacios en los extremos

            // Ruta donde se guardará el archivo en S3
            var key = $"pruebas/{filename}.pdf";

            var fileBytes = Convert.FromBase64String(fileFirmasPdf);
            using var originalPdfStream = new MemoryStream(fileBytes);

            using var updatedPdfStream = new MemoryStream();
            using (var pdfReader = new PdfReader(originalPdfStream))
            using (var pdfWriter = new PdfWriter(updatedPdfStream))
            using (var pdfDoc = new PdfDocument(pdfReader, pdfWriter))
            {
                var document = new Document(pdfDoc);

                // 3. Generar el código QR
                string qrContent = $"https://{_bucketName}.s3.{RegionEndpoint.GetBySystemName("us-east-2").SystemName}.amazonaws.com/{key}";
                var barcode = new BarcodeQRCode(qrContent);
                var qrImage = new iText.Layout.Element.Image(barcode.CreateFormXObject(pdfDoc));
                qrImage.SetWidth(100); // Ajusta el tamaño del QR
                qrImage.SetHeight(100);

                // 4. Insertar el QR en la primera página
                var page = pdfDoc.GetLastPage();
                var pageSize = page.GetPageSize();

                //var canvas = new iText.Kernel.Pdf.Canvas.PdfCanvas(page);
                //var rect = new iText.Kernel.Geom.Rectangle(50, 50, 100, 100); // Coordenadas (x, y, width, height)
                //var qrCanvas = new iText.Layout.Canvas(canvas, rect);
                //qrCanvas.Add(qrImage);

                // Calcular las coordenadas para la esquina inferior derecha
                float x = pageSize.GetWidth() - 100 - 50; // Ancho de la página - ancho del QR - margen derecho
                float y = 50; // Margen inferior

                // Establecer posición del QR
                qrImage.SetFixedPosition(x, y);

                // Añadir la imagen al documento
                document.Add(qrImage);
            }


            byte[] updatedPdfBytes = updatedPdfStream.ToArray();
            //string updatedBase64Pdf = Convert.ToBase64String(updatedPdfBytes);

            using var memoryStream = new MemoryStream(updatedPdfBytes);

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

            await createFileRepositoryBD(payload.fileName, fileUrl);

            return (fileUrl, "Succeeded");

        }
        catch (Exception ex)
        {

            return (null, $"Error : {ex.InnerException?.Message ?? ex.Message}");
        }

    }


    public async Task<(FileEntityCreate, string)> createFileNewRepository(CreateFileNewPayload payload)
    {

        try
        {

            //string fileFirmasPdf = await insertFirma(payload.base64File, payload.firmas);

            var fileBytesFirma = Convert.FromBase64String(payload.base64Firma.Replace("data:image/png;base64,", string.Empty));
            var imageData = ImageDataFactory.Create(fileBytesFirma);

            var filename = payload.fileName
                            .Replace(" ", "-") // Reemplaza espacios con guiones
                            .Trim(); // Elimina posibles espacios en los extremos

            // Ruta donde se guardará el archivo en S3
            var key = $"pruebas/{filename}.pdf";

            var fileBytes = Convert.FromBase64String(payload.base64File);
            using var originalPdfStream = new MemoryStream(fileBytes);

            using var updatedPdfStream = new MemoryStream();
            using (var pdfReader = new PdfReader(originalPdfStream))
            using (var pdfWriter = new PdfWriter(updatedPdfStream))
            using (var pdfDoc = new PdfDocument(pdfReader, pdfWriter))
            {
                var document = new Document(pdfDoc);

                //Generar el código QR
                string qrContent = $"https://{_bucketName}.s3.{RegionEndpoint.GetBySystemName("us-east-2").SystemName}.amazonaws.com/{key}";
                var barcode = new BarcodeQRCode(qrContent);
                var qrImage = new iText.Layout.Element.Image(barcode.CreateFormXObject(pdfDoc));
                qrImage.SetWidth(90); // Ajusta el tamaño del QR
                qrImage.SetHeight(90);


                // Crear una imagen iText con la data de la firma
                var signatureImage = new iText.Layout.Element.Image(imageData);
                var widtFirma = 140;
                var heightFirma = 89;
                signatureImage.SetWidth(widtFirma); // Ajustar ancho de la firma
                signatureImage.SetHeight(heightFirma); // Ajustar alto de la firma

                // Insertar el QR en la primera página
                var page = pdfDoc.GetLastPage();
                var pageSize = page.GetPageSize();
                var marginBottom = 100f; // Margen inferior
                var spacing = 20f; // Espacio entre marcos


                // Crear marcos rectangulares

                // Definir un solo marco grande que cubra toda el área inferior
                float frameWidth = pageSize.GetWidth() - 120; // Margen izquierdo + derecho (50 + 50)
                float frameHeight = 120f;
                float frameX = 60f; // Margen izquierdo
                float frameY = marginBottom;

                // Dibujar el marco principal
                var canvas = new PdfCanvas(page);
                canvas.Rectangle(frameX, frameY, frameWidth, frameHeight);
                canvas.SetStrokeColor(DeviceGray.BLACK);
                canvas.Stroke();

                ////Dibujar línea vertical central dentro del marco
                float centerX = frameX + (frameWidth / 3);
                canvas.MoveTo(centerX, frameY);
                canvas.LineTo(centerX, frameY + frameHeight);
                canvas.Stroke();

                float centerX2 = frameX + 2 * (frameWidth / 3);
                canvas.MoveTo(centerX2, frameY);
                canvas.LineTo(centerX2, frameY + frameHeight);
                canvas.Stroke();

                // Posicionar elementos dentro del marco unificado
                // - QR en la mitad izquierda
                //float qrX = pageSize.GetWidth()/2 - frameWidth/6 - 50; // Margen interno izquierdo
                float qrX = frameX + frameWidth / 6 - 45; // Margen interno izquierdo
                float qrY = frameY + frameHeight / 2 - 45; // Margen inferior interno
                qrImage.SetFixedPosition(pdfDoc.GetNumberOfPages(), qrX, qrY);

                // - Firma en la mitad derecha
                float signatureX = frameX + 5 * (frameWidth / 6) - widtFirma / 2; // Margen interno derecho (después de la línea central)
                float signatureY = frameY + frameHeight / 2 - heightFirma / 2;
                signatureImage.SetFixedPosition(pdfDoc.GetNumberOfPages(), signatureX, signatureY);

                // Agregar todo al documento
                document.Add(qrImage);
                document.Add(signatureImage);
            }


            byte[] updatedPdfBytes = updatedPdfStream.ToArray();
            //string updatedBase64Pdf = Convert.ToBase64String(updatedPdfBytes);

            using var memoryStream = new MemoryStream(updatedPdfBytes);

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

            var result = new FileEntityCreate();

            result.fileRuta = fileUrl;

            result.id = await createFileRepositoryBD(payload.fileName, fileUrl);

            return (result, "Succeeded");

        }
        catch (Exception ex)
        {

            return (null, $"Error : {ex.InnerException?.Message ?? ex.Message}");
        }

    }


    private async Task<string> insertFirma(string base64File, List<FirmasList> firmasList)
    {
        try
        {
            string file1;
            string fileModificado = null;

            file1 = base64File;
            int spaceBetweenSignatures = 0;

            foreach (var item in firmasList)
            {
                fileModificado = await insertFirmaDetalle(file1, item.ruta, spaceBetweenSignatures);
                file1 = fileModificado;
                spaceBetweenSignatures += 60;
            }

            return fileModificado; // Retornar el PDF modificado en Base64
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(ex.InnerException?.Message ?? ex.Message);
        }
    }


    private async Task<string> insertFirmaDetalle(string base64File, string firmaruta, int spaceBetweenSignatures)
    {
        try
        {
            byte[] pdfBytes = Convert.FromBase64String(base64File);
            using var originalPdfStream = new MemoryStream(pdfBytes);

            // 2. Descargar la imagen desde la URL de S3 y convertirla a Base64
            byte[] imageBytes = await DownloadImageFromUrlAsync(firmaruta);
            var imageData = ImageDataFactory.Create(imageBytes);

            int y = 50 + spaceBetweenSignatures;

            // 3. Crear un nuevo PDF donde se insertará la firma
            using var updatedPdfStream = new MemoryStream();
            using (var pdfReader = new PdfReader(originalPdfStream))
            using (var pdfWriter = new PdfWriter(updatedPdfStream))
            using (var pdfDoc = new PdfDocument(pdfReader, pdfWriter))
            {
                var document = new Document(pdfDoc);

                // Crear una imagen iText con la data de la firma
                var signatureImage = new iText.Layout.Element.Image(imageData);
                signatureImage.SetWidth(150); // Ajustar ancho de la firma
                signatureImage.SetHeight(50); // Ajustar alto de la firma

                // Insertar la firma en la última página del PDF
                var page = pdfDoc.GetLastPage();
                var pageSize = page.GetPageSize();
                var x = pageSize.GetWidth() / 2 - 75; // Centrar horizontalmente // Espacio entre cada firma
                signatureImage.SetFixedPosition(x, y);
                document.Add(signatureImage);
            }

            // 4. Convertir el PDF modificado a Base64
            //updatedPdfStream.Position = 0;
            //updatedPdfStream.Seek(0, SeekOrigin.Begin);

            byte[] updatedPdfBytes = updatedPdfStream.ToArray();
            string updatedBase64Pdf = Convert.ToBase64String(updatedPdfBytes);

            return updatedBase64Pdf; // Retornar el PDF modificado en Base64
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(ex.InnerException?.Message ?? ex.Message);
        }
    }



    private static async Task<byte[]> DownloadImageFromUrlAsync(string imageUrl)
    {
        using var httpClient = new HttpClient();
        var response = await httpClient.GetAsync(imageUrl);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsByteArrayAsync();
    }


    private async Task<int> createFileRepositoryBD(string fileName, string rutaFile)
    {
        try
        {

            using (var connection = new NpgsqlConnection(_connectionString))
            //using (var connection = new NpgsqlConnection(_connectionString))
            {

                connection.Open();

                string query = @" INSERT INTO metroli.mst_pdf
                                       (p_descripcion,
                                        p_ruta,
                                        p_estado,
                                        p_fecha,
                                        p_firmado)
                                     VALUES(
                                        @descripcion,
                                        @ruta,
                                        @estado,
                                        @fecha,
                                        false); 
                                        SELECT LASTVAL();";


                var parameters = new
                {
                    descripcion = fileName,
                    ruta = rutaFile,
                    estado = true,
                    fecha = DateTime.UtcNow.AddHours(-5)
                };

                var result = await connection.QueryFirstOrDefaultAsync<int>(query, parameters);

                return result;

            }

        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(ex.InnerException?.Message ?? ex.Message);
        }
    }

    public async Task<bool> existeFileRepositoryId(int id)
    {
        try
        {

            using (var connection = new NpgsqlConnection(_connectionString))
            {

                connection.Open();

                string query = @" SELECT count(*)
                                     FROM metroli.mst_pdf
                                        WHERE p_id=@valor and p_estado=true;";


                var parameters = new
                {
                    valor = id
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

    public async Task<string> existeFileRepositoryIdName(int id)
    {
        try
        {

            using (var connection = new NpgsqlConnection(_connectionString))
            {

                connection.Open();

                string query = @" SELECT p_descripcion
                                     FROM metroli.mst_pdf
                                        WHERE p_id=@valor and p_estado=true;";


                var parameters = new
                {
                    valor = id
                };

                var result = await connection.QueryFirstOrDefaultAsync<string>(query, parameters);

                return result;

            }

        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(ex.InnerException?.Message ?? ex.Message);
        }
    }


    public async Task<bool> existeFileRepositoryName(string value)
    {
        try
        {
            using(var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();

                string query = @" SELECT count(*)
                                     FROM metroli.mst_pdf
                                        WHERE p_descripcion=@valor and p_estado=true;";

                var parameters = new
                {
                    valor = value
                };

                var result = await connection.QueryFirstOrDefaultAsync<int>(query, parameters);

                if (result > 0)
                    return true;

                else
                    return false;

            }

        }
        catch(Exception ex)
        {
            throw new InvalidOperationException(ex.InnerException.Message ?? ex.Message);
        }


    }


    public async Task<(bool, string)> deleteFileRepository(DeleteFirmaPayload payload)
    {
        try
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            //using (var connection = new NpgsqlConnection(_connectionString))
            {

                connection.Open();

                string query = @" UPDATE metroli.mst_pdf
                                  SET p_estado = @estado
                                     WHERE p_id = @id;";


                var parameters = new
                {
                    estado = false,
                    id = payload.id
                };

                var result = await connection.QueryFirstOrDefaultAsync<int>(query, parameters);

            }


            return (true, "Succeeded");
        }
        catch (Exception ex)
        {

            return (false, $"Error : {ex.InnerException?.Message ?? ex.Message}");
        }
    }

    public async Task<List<FileEntity>> listarFileRepository(string filtro)
    {
        try
        {
            var result = new List<FileEntity>();

            using (var connection = new NpgsqlConnection(_connectionString))
            {
                string query = @"
                        SELECT 
                            p_id as id,
                            p_descripcion as fileName,
                            p_ruta as fileRuta,
                            p_fecha as fecha,
                            p_estado as estado,
                            p_firmado as firmado
                        FROM metroli.mst_pdf
                        WHERE p_estado=true
                    ";


                DynamicParameters parameters = new DynamicParameters();

                if (!string.IsNullOrEmpty(filtro))
                {
                    query += " AND p_descripcion LIKE @archivo";
                    parameters.Add("archivo", $"%{filtro}%");
                }

                result = connection.Query<FileEntity>(query, parameters).ToList();

            }

            return result;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(ex.InnerException?.Message ?? ex.Message);
        }
    }


    public async Task<string> updateFileRepository(UpdateFilePayload payload, string fileName)
    {
        try
        {
            var fileBytes = Convert.FromBase64String(payload.base64File);
            
            using var memoryStream = new MemoryStream(fileBytes);

            var filename = fileName
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
                ContentType = $"application/pdf"
            };

            // Subir el archivo a S3
            var transferUtility = new TransferUtility(_s3Client);
            await transferUtility.UploadAsync(request);

            // Devolver la URL del archivo guardado
            string fileUrl = $"https://{_bucketName}.s3.{RegionEndpoint.GetBySystemName("us-east-2").SystemName}.amazonaws.com/{key}";

            await updateFirmaRepositoryBD(fileUrl, payload.id);

            return fileUrl;
        }
        catch(Exception ex)
        {
            throw new InvalidOperationException(ex.InnerException?.Message ?? ex.Message);
        }

    }


    public async Task updateFirmaRepositoryBD(string fileUrl, int id)
    {
        try
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                connection.Open();

                string query = @" UPDATE metroli.mst_pdf
                                     SET p_ruta = @valor,
                                          p_firmado = true,
                                          p_fecha = @fecha
                                        WHERE p_id=@id and p_estado=true;";

                var parameters = new
                {
                    valor = fileUrl,
                    id = id,
                    fecha = DateTime.UtcNow.AddHours(-5)
                };

                var result = await connection.QueryFirstOrDefaultAsync<int>(query, parameters);

            }
        }
        catch(Exception ex)
        {
            throw new InvalidOperationException(ex.InnerException?.Message ?? ex.Message);
        }
    }
}
