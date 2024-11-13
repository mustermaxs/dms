using Microsoft.Extensions.Configuration;
using Minio;
using Minio.DataModel.Args;
using OcrService.Configs;

namespace OcrService;

public class FileStorage
{
    private readonly IMinioClient _minioClient;
    private readonly string _bucketName;

    public FileStorage(IMinioClient minioClient, FileStorageConfig config)
    {
        _minioClient = minioClient;
        _bucketName = config.BucketName ?? throw new InvalidOperationException();
    }

    private async Task EnsureBucketExistsAsync()
    {
        try
        {
            var beArgs = new BucketExistsArgs()
                .WithBucket(_bucketName);
            bool found = await _minioClient.BucketExistsAsync(beArgs).ConfigureAwait(false);

            if (!found)
            {
                var mbArgs = new MakeBucketArgs()
                    .WithBucket(_bucketName);
                await _minioClient.MakeBucketAsync(mbArgs).ConfigureAwait(false);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error setting up MinIO bucket: {ex.Message}");
            throw;
        }
    }


    public async Task<MemoryStream> GetFileStreamAsync(string fileName)
    {
        try
        {
            await EnsureBucketExistsAsync();
            MemoryStream fileStream = new MemoryStream();
            fileStream.Position = 0;
            await fileStream.FlushAsync();
            StatObjectArgs statObjectArgs = new StatObjectArgs()
                .WithBucket(_bucketName)
                .WithObject(fileName);
            await _minioClient.StatObjectAsync(statObjectArgs);
            // BUG Minio.Exceptions.BucketNotFoundException: Exception of type 'Minio.Exceptions.BucketNotFoundException'
            // wird von DMS REST FileStorage aber gefunden
            var args = new GetObjectArgs()
                .WithBucket(_bucketName)
                .WithObject(fileName)
                .WithCallbackStream((stream) => { stream.CopyTo(fileStream); });

            var objRes = await _minioClient.GetObjectAsync(args).ConfigureAwait(false);
            return fileStream;
        }
        catch (Minio.Exceptions.BucketNotFoundException ex)
        {
            Console.WriteLine($"Bucket '{_bucketName}' not found: {ex.Message}");
            throw;
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error getting file stream: {e.Message}");
            throw;
        }
    }
}