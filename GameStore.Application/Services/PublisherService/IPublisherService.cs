using GameStore.Application.DTOs.PublisherDtos;

namespace GameStore.Application.Services.PublisherService;
public interface IPublisherService
{
    Task<PublisherDto> AddPublisherAsync(AddPublisherDto publisherDto);

    Task<PublisherDto> GetPublisherByNameAsync(string companyName, bool includeDeleted = false);

    Task<IEnumerable<PublisherDto>> GetAllPublishersAsync(bool includeDeleted = false);

    Task<PublisherDto> GetPublisherByGameKeyAsync(string gameKey, bool includeDeleted = false);

    Task UpdatePublisherAsync(PublisherDto publisherDto, bool includeDeleted = false);

    Task DeletePublisherAsync(string id);
}
