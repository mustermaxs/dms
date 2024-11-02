namespace DMS.Application.Interfaces;

public interface IFileStorage
{
    Task<string> SaveFileAsync(Guid id, Stream fileStream);
    Task<Stream> GetFileAsync(Guid id);
    Task<bool> DeleteFileAsync(Guid id);
    Task DeleteAllFilesAsync();
}