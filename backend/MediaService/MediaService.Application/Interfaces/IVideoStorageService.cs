namespace MediaService.Application.Interfaces;

public interface IVideoStorageService
{
    Task<string> UploadAsync(Stream content, string bucket, string objectKey, string contentType, CancellationToken ct);
    Task<string> DownloadToTempFileAsync(string storagePath, CancellationToken ct);
    Task DeleteAsync(string storagePath, CancellationToken ct);
}