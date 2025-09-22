using GameStore.Domain.Entities;

namespace GameStore.Application.Services.GameServices.Pipeline.GamesFilterSteps;
public class LimitStep(int pageNumber, string? pageSize) : IPipelineStep<Game>
{
    public int TotalNumber { get; private set; }

    public IQueryable<Game> Process(IQueryable<Game> input)
    {
        TotalNumber = input.Count();

        if (pageNumber < 1)
        {
            pageNumber = 1;
        }

        return int.TryParse(pageSize, out int pageItemsCount)
            ? input
                .Take(pageItemsCount * pageNumber)
            : input;
    }
}
