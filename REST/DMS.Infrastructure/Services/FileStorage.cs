using System.IO.Compression;
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
                    .WithCallbackStream((stream) =>
                    {
                        stream.CopyTo(fileStream);
                    });
                
                var objRes = await _minioClient.GetObjectAsync(args).ConfigureAwait(false);
                return fileStream;
            }
            catch (Minio.Exceptions.BucketNotFoundException ex)
            {
                Console.WriteLine($"Bucket '{_bucketName}' not found: {ex.Message}");
                throw;
            }
        }

        public Task<Stream> GetFileAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> DeleteFileAsync(Guid id)
        {
            try
            {
                await EnsureBucketExistsAsync();
                var args = new RemoveObjectArgs()
                    .WithBucket(_bucketName)
                    .WithObject(id.ToString());
            
                await _minioClient.RemoveObjectAsync(args);

                return true;
            }
            catch (Exception e)
            {
                return false;
                throw;
            }

        }

        public async Task DeleteAllFilesAsync()
        {
            try
            {
                await EnsureBucketExistsAsync();
                var args = new ListObjectsArgs()
                    .WithBucket(_bucketName)
                    .WithRecursive(true);
                
                var fileNames = new List<string>();
            
                await foreach (var file in _minioClient.ListObjectsEnumAsync(args).ConfigureAwait(false))
                {
                    fileNames.Add(file.Key);
                }

                foreach (var fileName in fileNames)
                {
                    await DeleteFileAsync(Guid.Parse(fileName));
                }
            }
            catch (Exception e)
            {
                throw;
            }

        }
    }
}
