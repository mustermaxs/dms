using DMS.Application.Interfaces;

namespace DMS.Infrastructure.Services;

public class FileStorage : IFileStorage
{
    public Task<string> SaveFileAsync(Guid id, Stream fileStream)
    {
        throw new NotImplementedException();
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