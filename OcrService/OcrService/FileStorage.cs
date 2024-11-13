using Microsoft.Extensions.Configuration;
using Minio;
using Minio.DataModel.Args;

namespace OcrService;

public class FileStorage
{
    private readonly IMinioClient _minioClient;
    private readonly string _bucketName;

    public FileStorage(IMinioClient minioClient, MinioConfig config)
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
    
    public async Task<Stream> GetFileStreamAsync(string fileName)
    {
        try
        {
            await EnsureBucketExistsAsync();
            Stream fileStream = new MemoryStream();
            
            var args = new GetObjectArgs()
                .WithBucket(_bucketName)
                .WithObject(fileName)
                .WithCallbackStream((stream) =>
                {
                    stream.CopyTo(fileStream);
                });
            await _minioClient.GetObjectAsync(args).ConfigureAwait(false);
            
            return fileStream;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}