namespace DMS.Domain.ValueObjects
{
    public class FileType: ValueObject
    {
        public FileType(string extension)
        {
            Extension = GetFileExtensionFromMimeType(extension);
        }

        private string GetFileExtensionFromMimeType(string mimeType)
        {
            switch (mimeType)
            {
                case "text/plain":
                    return ".txt";
                case "application/pdf":
                    return ".pdf";
                case "image/jpeg":
                    return ".jpeg";
                default:
                    return mimeType;
            }
        }
        public string Extension { get; private set; }

        protected override IEnumerable<object> GetAtomicValues()
        {
            yield return Extension;
        }

        public override string ToString()
        {
            return Extension;
        }
    }
}