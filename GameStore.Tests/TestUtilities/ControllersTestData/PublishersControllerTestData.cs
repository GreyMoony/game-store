using GameStore.Application.DTOs.PublisherDtos;

namespace GameStore.Tests.TestUtilities.ControllersTestData;
public static class PublishersControllerTestData
{
    public static PublisherDto PublisherDto => new()
    {
        Id = Guid.NewGuid().ToString(),
        CompanyName = "Publisher name",
        Description = "Publisher description",
        HomePage = "http://www.google.com",
    };

    public static List<PublisherDto> ListOfPublisherDtos =>
        [
            new()
            {
                Id = Guid.NewGuid().ToString(),
                CompanyName = "Publisher 1",
                Description = "Description 1",
                HomePage = "http://www.google.com",
            },
            new()
            {
                Id = Guid.NewGuid().ToString(),
                CompanyName = "Publisher 2",
                Description = "Description 2",
                HomePage = "http://www.google.com",
            },
        ];
}
