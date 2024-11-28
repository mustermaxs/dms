namespace OcrService.DTOs
{
    public enum ProcessStatus
    {
        Failed = 0,
        Succeeded = 1,
    };

    public class OcrProcessedDocumentDto
    {
        public Guid Id { get; set; }
        public string Content { get; set; }
        public ProcessStatus Status { get; set; }
    } 
}