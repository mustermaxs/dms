namespace OcrService.Configs;

public class FileStorageConfig
{
    public required string Endpoint { get; set; }
    public required string AccessKey { get; set; }
    public required string SecretKey { get; set; }
    public required string BucketName { get; set; }
    public required ushort Port { get; set; }
}