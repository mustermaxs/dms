using DMS.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;
using MinioConfig = DMS.Infrastructure.Configs.MinioConfig;

namespace DMS.Infrastructure.Services;

public class FileStorage : IFileStorage
{
    private  IMinioClient _minioClient;
    private string BucketName { get; }

    public FileStorage(string accessKey, string secretKey, string endpoint, string bucketName)
    {
        BucketName = bucketName;
        _minioClient = new MinioClient()
            .WithCredentials(accessKey, secretKey)
            .WithEndpoint(endpoint)
            .Build();
    }

    private async Task SetupMinioClient()
    {
        try
        {
            var beArgs = new BucketExistsArgs()
                .WithBucket(BucketName);
            bool found = await _minioClient.BucketExistsAsync(beArgs).ConfigureAwait(false);
            if (!found)
            {
                var mbArgs = new MakeBucketArgs()
                    .WithBucket(BucketName);
                await _minioClient.MakeBucketAsync(mbArgs).ConfigureAwait(false);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task<string> SaveFileAsync(Guid id, Stream fileStream)
    {
        await SetupMinioClient();
        var args = new PutObjectArgs()
            .WithBucket(BucketName)
            .WithObject(id.ToString())
            .WithStreamData(fileStream)
            .WithObjectSize(fileStream.Length)
            .WithContentType("application/pdf");
        await _minioClient.PutObjectAsync(args).ConfigureAwait(false);
        
        return $"{BucketName}/{id.ToString()}";
    }

    public Task<Stream> GetFileAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteFileAsync(Guid id)
    {
        throw new NotImplementedException();
    }
}