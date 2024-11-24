namespace DMS.Infrastructure.Models;

public class FileTypeModel(string name)
{
    public string Name { get; } = GetExtensionFromName(name) ?? throw new ArgumentException("Invalid file extension.", nameof(name));

    public static FileTypeModel GetFileTypeFromExtension(string fileName)
    {
        var extension = GetExtensionFromName(fileName);
        return extension switch
        {
            ".pdf" => new FileTypeModel("1.pdf"),
            ".docx" => new FileTypeModel("1.docx"),
            ".txt" => new FileTypeModel("1.txt"),
            _ => throw new ArgumentOutOfRangeException(nameof(extension), extension, null)
        };
    }

    private static string GetExtensionFromName(string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            throw new ArgumentException("File name cannot be null or empty", nameof(fileName));

        var extension = Path.GetExtension(fileName)?.ToLower();
        if (string.IsNullOrEmpty(extension))
            throw new ArgumentException("File must have a valid extension.", nameof(fileName));

        return extension;
    }

    public override string ToString()
    {
        return Name;
    }
}