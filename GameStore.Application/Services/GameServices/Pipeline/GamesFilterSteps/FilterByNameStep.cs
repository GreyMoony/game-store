using GameStore.Domain.Entities;

namespace GameStore.Application.Services.GameServices.Pipeline.GamesFilterSteps;
#pragma warning disable CA1862, CA1311
public class FilterByNameStep(string name) : IPipelineStep<Game>
{
    public IQueryable<Game> Process(IQueryable<Game> input)
    {
        return input.Where(g => g.Name.ToLower().Contains(name.ToLower()));
    }
}
