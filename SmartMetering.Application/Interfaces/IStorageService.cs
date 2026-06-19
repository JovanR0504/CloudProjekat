namespace SmartMetering.Application.Interfaces;

public interface IStorageService
{
    Task<string> UploadBlobAsync(string containerName, string blobName, Stream content, string contentType);
    Task<Stream> DownloadBlobAsync(string containerName, string blobName);
    Task DeleteBlobAsync(string containerName, string blobName);
}
