namespace DMS.Infrastructure.Configs;

public class MinioConfig
{
    public string Endpoint { get; set; }
    public int Port { get; set; }
    public string AccessKey { get; set; }
    public string SecretKey { get; set; }
    public bool UseSSL { get; set; }
    public string BucketName { get; set; }
}
