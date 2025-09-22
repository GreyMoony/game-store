using GameStore.Domain.Entities;

namespace GameStore.Application.Services.GameServices.Pipeline.GamesFilterSteps;
public class FilterByPublishDateStep(string publishDate) : IPipelineStep<Game>
{
    public IQueryable<Game> Process(IQueryable<Game> input)
    {
        var now = DateTime.UtcNow;
        return publishDate switch
        {
            PublishDateOptions.Week => input.Where(g => g.CreatedAt >= now.AddDays(-7)),
            PublishDateOptions.Month => input.Where(g => g.CreatedAt >= now.AddMonths(-1)),
            PublishDateOptions.Year => input.Where(g => g.CreatedAt >= now.AddYears(-1)),
            PublishDateOptions.TwoYears => input.Where(g => g.CreatedAt >= now.AddYears(-2)),
            PublishDateOptions.ThreeYears => input.Where(g => g.CreatedAt >= now.AddYears(-3)),
            _ => input,
        };
    }
}
