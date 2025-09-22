using GameStore.Application.DTOs.GameDtos;

namespace GameStore.Application.Services.BlobStorageService;
public interface IBlobStorageService
{
    Task<string> UploadImageAsync(string base64Image, string gameKey);

    Task<GameImageDto?> GetImageAsync(string blobName);

    Task<bool> DeleteImageAsync(string blobName);
}
