using DMS.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;
using MinioConfig = DMS.Infrastructure.Configs.MinioConfig;

namespace DMS.Infrastructure.Services
{
    public class FileStorage : IFileStorage
    {
        private readonly IMinioClient _minioClient;
        private readonly string _bucketName;

        public FileStorage(IMinioClient minioClient, IConfiguration config)
        {
            _minioClient = minioClient;
            _bucketName = config["MinIO:BucketName"] ?? throw new InvalidOperationException();
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

        public async Task<string> SaveFileAsync(Guid id, Stream fileStream)
        {
            try {
                await EnsureBucketExistsAsync();

                var args = new PutObjectArgs()
                    .WithBucket(_bucketName)
                    .WithObject(id.ToString())
                    .WithStreamData(fileStream)
                    .WithObjectSize(fileStream.Length)
                    .WithContentType("application/pdf");

                await _minioClient.PutObjectAsync(args).ConfigureAwait(false);

                return $"{id}";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving file to MinIO: {ex.Message}");
                throw;
            }
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
}
