using MediaService.Application.Interfaces;
using MediaService.Domain.Constants;
using Minio;
using Minio.DataModel.Args;

namespace MediaService.Infrastructure.Services;

public sealed class MinioStorageService(IMinioClient minioClient) : IVideoStorageService
{
    public async Task<string> UploadAsync(Stream content, string bucket, string objectKey, string contentType, CancellationToken ct)
    {
        await EnsureBucketExistsAsync(bucket, ct);

        content.Position = 0;
        await minioClient.PutObjectAsync(new PutObjectArgs()
            .WithBucket(bucket)
            .WithObject(objectKey)
            .WithStreamData(content)
            .WithObjectSize(content.Length)
            .WithContentType(contentType), ct);

        return $"{bucket}/{objectKey}";
    }

    public async Task<string> DownloadToTempFileAsync(string storagePath, CancellationToken ct)
    {
        var (bucket, key) = ParseStoragePath(storagePath);
        var tempPath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}{Path.GetExtension(key)}");

        await using var fileStream = File.Create(tempPath);
        await minioClient.GetObjectAsync(new GetObjectArgs()
            .WithBucket(bucket)
            .WithObject(key)
            .WithCallbackStream(stream => stream.CopyTo(fileStream)), ct);

        return tempPath;
    }

    public async Task DeleteAsync(string storagePath, CancellationToken ct)
    {
        var (bucket, key) = ParseStoragePath(storagePath);
        await minioClient.RemoveObjectAsync(new RemoveObjectArgs().WithBucket(bucket).WithObject(key), ct);
    }

    private async Task EnsureBucketExistsAsync(string bucket, CancellationToken ct)
    {
        var exists = await minioClient.BucketExistsAsync(new BucketExistsArgs().WithBucket(bucket), ct);
        if (!exists)
            await minioClient.MakeBucketAsync(new MakeBucketArgs().WithBucket(bucket), ct);

        if (MediaBuckets.Public.Contains(bucket))
        {
            var policy = $$"""
        {
          "Version": "2012-10-17",
          "Statement": [
            {
              "Effect": "Allow",
              "Principal": { "AWS": ["*"] },
              "Action": ["s3:GetObject"],
              "Resource": ["arn:aws:s3:::{{bucket}}/*"]
            }
          ]
        }
        """;
            await minioClient.SetPolicyAsync(new SetPolicyArgs().WithBucket(bucket).WithPolicy(policy), ct);
        }
    }

    private static (string Bucket, string Key) ParseStoragePath(string storagePath)
    {
        var idx = storagePath.IndexOf('/');
        return (storagePath[..idx], storagePath[(idx + 1)..]);
    }
}