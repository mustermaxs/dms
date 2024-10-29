namespace DMS.Application.Services;

public class FileHelper
{
    public Stream FromBase64ToStream(string base64String)
    {
        byte[] fileBytes = Convert.FromBase64String(base64String);
        return new MemoryStream(fileBytes);
    }
}