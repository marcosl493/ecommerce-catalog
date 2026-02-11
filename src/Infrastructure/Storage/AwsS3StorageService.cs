using Application.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Storage;

public sealed class AwsS3StorageService : IStorageService
{
    private readonly string _baseUrl;
    private readonly string _defaultBucket;
    private readonly string _uploadsPath;

    public AwsS3StorageService(IConfiguration configuration)
    {
        _uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
        Directory.CreateDirectory(_uploadsPath);

        _baseUrl = configuration.GetValue<string>("Storage:BaseUrl") ?? "file://";
        _defaultBucket = configuration.GetValue<string>("Storage:Bucket") ?? "local-bucket";
    }

    public Task DeleteAsync(string bucket, string key, CancellationToken cancellationToken = default)
    {
        var filePath = Path.Combine(_uploadsPath, bucket, key);
        if (File.Exists(filePath))
            File.Delete(filePath);

        return Task.CompletedTask;
    }

    public Task<Uri> UploadAsync(string bucket, string key, byte[] content, string contentType = "application/octet-stream", CancellationToken cancellationToken = default)
    {
        var bucketPath = Path.Combine(_uploadsPath, bucket ?? _defaultBucket);
        Directory.CreateDirectory(bucketPath);

        var filePath = Path.Combine(bucketPath, key);
        File.WriteAllBytes(filePath, content);

        var uri = new Uri(Path.Combine(_baseUrl, bucket ?? _defaultBucket, key));
        return Task.FromResult(uri);
    }
}
