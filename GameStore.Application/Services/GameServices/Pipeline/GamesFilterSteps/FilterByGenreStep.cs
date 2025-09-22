using GameStore.Application.Helpers;
using GameStore.Domain.Entities;

namespace GameStore.Application.Services.GameServices.Pipeline.GamesFilterSteps;
public class FilterByGenreStep(List<string> genres) : IPipelineStep<Game>
{
    public IQueryable<Game> Process(IQueryable<Game> input)
    {
        var parsedGuids = IdParser.GetGuids(genres);

        return input
            .Where(game => game.GameGenres.Any(gg => parsedGuids.Contains(gg.GenreId)));
    }
}
