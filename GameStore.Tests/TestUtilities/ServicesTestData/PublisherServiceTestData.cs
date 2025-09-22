using GameStore.Application.DTOs.PublisherDtos;
using GameStore.Domain.Entities;

namespace GameStore.Tests.TestUtilities.ServicesTestData;

#pragma warning disable SA1010
public static class PublisherServiceTestData
{
    public static AddPublisherDto AddPublisherDto => new()
    {
        CompanyName = "Publisher to add",
        Description = "description",
        HomePage = "http://google.com",
    };

    public static List<Publisher> PublisherList =>
    [
        new() { CompanyName = "Test publisher 1" },
        new() { CompanyName = "Test publisher 1" }
    ];

    public static Publisher Publisher => new()
    {
        Id = Guid.NewGuid(),
        CompanyName = "Publisher",
        Description = "description",
        HomePage = "http://google.com",
    };

    public static PublisherDto PublisherDto => new()
    {
        Id = "11111111-1111-1111-1111-111111111111",
        CompanyName = "Publisher to update",
        Description = "description",
        HomePage = "http://google.com",
    };
}
