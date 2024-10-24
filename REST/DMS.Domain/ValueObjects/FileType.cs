namespace DMS.Domain.ValueObjects
{
    public class FileType(string name) : ValueObject
    {
        public string Name { get; } = GetExtensionFromName(name) ?? throw new ArgumentException("Invalid file extension.", nameof(name));

        public static FileType GetFileTypeFromExtension(string fileName)
        {
            var extension = GetExtensionFromName(fileName);
            return extension switch
            {
                ".pdf" => new FileType("pdf"),
                ".docx" => new FileType("docx"),
                ".txt" => new FileType("txt"),
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

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Name;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}