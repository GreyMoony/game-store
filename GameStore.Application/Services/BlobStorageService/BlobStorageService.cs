using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using GameStore.Application.DTOs.GameDtos;
using Microsoft.Extensions.Caching.Memory;

namespace GameStore.Application.Services.BlobStorageService;
public class BlobStorageService(BlobServiceClient blobServiceClient, IMemoryCache cache) : IBlobStorageService
{
    public const string ContainerName = "game-images";

    private readonly BlobContainerClient _containerClient = blobServiceClient.GetBlobContainerClient(ContainerName);

    private readonly IMemoryCache _cache = cache;

    public async Task<string> UploadImageAsync(string base64Image, string gameKey)
    {
        if (string.IsNullOrEmpty(base64Image))
        {
            return string.Empty;
        }

        string imageFormat = base64Image.Split(';')[0].Split('/')[1];

        byte[] imageBytes = Convert.FromBase64String(base64Image.Split(',')[1]);

        string blobName = $"{gameKey}.{imageFormat}";
        var blobClient = _containerClient.GetBlobClient(blobName);

        using var stream = new MemoryStream(imageBytes);

        var headers = new BlobHttpHeaders { ContentType = $"image/{imageFormat}" };
        var metadata = new Dictionary<string, string>
        {
            { "GameKey", gameKey },
            { "Format", imageFormat },
        };

        await blobClient.UploadAsync(stream, new BlobUploadOptions
        {
            HttpHeaders = headers,
            Metadata = metadata,
        });

        _cache.Remove(blobName); // Invalidate cache
        return blobName;
    }

    public async Task<GameImageDto?> GetImageAsync(string blobName)
    {
        if (_cache.TryGetValue(blobName, out GameImageDto image))
        {
            return image;
        }

        var blobClient = _containerClient.GetBlobClient(blobName);

        if (!await blobClient.ExistsAsync())
        {
            return null;
        }

        var download = await blobClient.DownloadContentAsync();
        image = new GameImageDto
        {
            File = download.Value.Content.ToArray(),
            ContentType = download.Value.Details.ContentType ?? "application/octet-stream",
        };

        _cache.Set(blobName, image, new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5),
            Size = 1,
        });

        return image;
    }

    public async Task<bool> DeleteImageAsync(string blobName)
    {
        var blobClient = _containerClient.GetBlobClient(blobName);
        var result = await blobClient.DeleteIfExistsAsync();

        if (result)
        {
            _cache.Remove(blobName);
        }

        return result;
    }
}
