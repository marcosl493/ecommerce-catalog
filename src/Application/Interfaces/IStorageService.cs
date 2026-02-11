namespace Application.Interfaces;

public interface IStorageService
{
    /// <summary>
    /// Uploads content to the specified bucket/key and returns the public Uri to the stored object.
    /// </summary>
    Task<Uri> UploadAsync(string bucket, string key, byte[] content, string contentType = "application/octet-stream", CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes object by bucket and key.
    /// </summary>
    Task DeleteAsync(string bucket, string key, CancellationToken cancellationToken = default);
}
