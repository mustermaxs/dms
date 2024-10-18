namespace DMS.Domain.Exceptions;

public class UploadDocumentException : Exception
{
    public UploadDocumentException(){}
    public UploadDocumentException(string message) : base(message){}
    public UploadDocumentException(string message, Exception innerException) : base(message, innerException){}
}