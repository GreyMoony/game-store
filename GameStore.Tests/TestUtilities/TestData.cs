using GameStore.Domain.Entities;

namespace GameStore.Tests.TestUtilities;
public class TestData
{
    public List<Genre> Genres { get; set; }

    public List<Platform> Platforms { get; set; }

    public List<Game> Games { get; set; }

    public List<Publisher> Publishers { get; set; }

    public List<Order> Orders { get; set; }

    public List<Comment> Comments { get; set; }
}
