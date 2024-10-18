namespace DMS.Domain.ValueObjects;

public class FileType : ValueObject
{
    private string _name;
    public string Name
    {
        get => _name;
        set => _name = GetExtensionFromName(value);
    }
    public FileType() {}

    public FileType(string name)
    {
        _name = GetExtensionFromName(name);
    }

    public static FileType GetFileTypeFromExtension(string name)
    {
        var extension = Path.GetExtension(name);
        return extension switch
        {
            ".pdf" => new FileType("pdf"),
            ".docx" => new FileType("docx"),
            ".txt" => new FileType("txt"),
            _ => throw new ArgumentOutOfRangeException(nameof(extension), extension, null)
        };
    }

    public static string GetExtensionFromName(string name)
    {
        return Path.GetExtension(name);
    }

    protected override IEnumerable<object> GetAtomicValues()
    {
       yield return _name;
    }
}