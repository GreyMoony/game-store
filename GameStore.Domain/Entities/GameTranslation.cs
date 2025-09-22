namespace GameStore.Domain.Entities;
public class GameTranslation
{
    public Guid GameId { get; set; }

    public string LanguageCode { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public Game Game { get; set; }
}
