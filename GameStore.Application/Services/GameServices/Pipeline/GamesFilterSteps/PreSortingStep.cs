using GameStore.Domain.Entities;

namespace GameStore.Application.Services.GameServices.Pipeline.GamesFilterSteps;
public class PreSortingStep(string? sortBy) : IPipelineStep<Game>
{
    public IQueryable<Game> Process(IQueryable<Game> input)
    {
        if (string.IsNullOrEmpty(sortBy))
        {
            return input.OrderByDescending(g => g.CreatedAt); // Default sorting by "New"
        }

        return sortBy switch
        {
            SortingOptions.MostCommented => input.OrderByDescending(g => g.Comments.Count),
            SortingOptions.MostPopular => input.OrderByDescending(g => g.ViewCount),
            SortingOptions.PriceAsc => input.OrderBy(g => g.Price),
            SortingOptions.PriceDesc => input.OrderByDescending(g => g.Price),
            _ => input.OrderByDescending(g => g.CreatedAt), // Default to "New"
        };
    }
}
