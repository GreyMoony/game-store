using GameStore.Domain.Entities;

namespace GameStore.Application.Services.GameServices.Pipeline.GamesFilterSteps;
public class FilterByMinPriceStep(double minPrice) : IPipelineStep<Game>
{
    public IQueryable<Game> Process(IQueryable<Game> input)
    {
        return input.Where(g => g.Price >= minPrice);
    }
}
