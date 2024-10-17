namespace DMS.Domain.ValueObjects;

public class FileType
{
    public string Name { get; set; }

    public FileType(string name)
    {
        Name = name;
    }

    public static FileType GetFileTypeFromExtension(string extension)
    {
        return extension switch
        {
            ".pdf" => new FileType("pdf"),
            ".docx" => new FileType("docx"),
            ".txt" => new FileType("txt"),
            _ => throw new ArgumentOutOfRangeException(nameof(extension), extension, null)
        };
    }
}