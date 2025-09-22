using GameStore.Application.Helpers;
using GameStore.Domain.Entities;

namespace GameStore.Application.Services.GameServices.Pipeline.GamesFilterSteps;
public class FilterByPlatformsStep(List<string> platforms) : IPipelineStep<Game>
{
    public IQueryable<Game> Process(IQueryable<Game> input)
    {
        var parsedGuids = IdParser.GetGuids(platforms);

        return input.Where(g => g.GamePlatforms.Any(gp => parsedGuids.Contains(gp.PlatformId)));
    }
}
