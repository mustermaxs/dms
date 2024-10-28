namespace DMS.Application.Exceptions;

public class UploadDocumentException : Exception
{
    public UploadDocumentException(){}
    public UploadDocumentException(string message) : base(message){}
    public UploadDocumentException(string message, Exception innerException) : base(message, innerException){}
}