using DMS.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;
using MinioConfig = DMS.Infrastructure.Configs.MinioConfig;

namespace DMS.Infrastructure.Services;

public class FileStorage : IFileStorage
{
    private readonly IMinioClient _minioClient;
    private IMediator _mediator { get; set;} 
    private MinioConfig _config { get; set; }

    public FileStorage(IMediator mediator, IMinioClient minioClient, MinioConfig config)
    {
        _minioClient = minioClient;
        _mediator = mediator;
        _config = config;
    }

    private async Task SetupMinioClient()
    {
        try
        {
            var beArgs = new BucketExistsArgs()
                .WithBucket(_config.BucketName);
            bool found = await _minioClient.BucketExistsAsync(beArgs).ConfigureAwait(false);
            if (!found)
            {
                var mbArgs = new MakeBucketArgs()
                    .WithBucket(_config.BucketName);
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
            .WithBucket(_config.BucketName)
            .WithObject(id.ToString())
            .WithFileName(id.ToString())
            .WithContentType("application/pdf")
            .WithStreamData(fileStream);
        await _minioClient.PutObjectAsync(args).ConfigureAwait(false);
        
        return $"{_config.BucketName}/{id.ToString()}";
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