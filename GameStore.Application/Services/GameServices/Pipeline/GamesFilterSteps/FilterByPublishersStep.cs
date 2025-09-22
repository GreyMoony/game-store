using GameStore.Application.Helpers;
using GameStore.Domain.Entities;

namespace GameStore.Application.Services.GameServices.Pipeline.GamesFilterSteps;

public class FilterByPublishersStep(List<string> publishers) : IPipelineStep<Game>
{
    public IQueryable<Game> Process(IQueryable<Game> input)
    {
        var parsedGuids = IdParser.GetGuids(publishers);

        return input.Where(g => parsedGuids.Contains(g.PublisherId));
    }
}