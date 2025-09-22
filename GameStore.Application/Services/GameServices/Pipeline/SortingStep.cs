using GameStore.Domain.Entities;

namespace GameStore.Application.Services.GameServices.Pipeline;
public class SortingStep(string? sortBy) : IPipelineStep<ICommonGame>
{
    public IQueryable<ICommonGame> Process(IQueryable<ICommonGame> input)
    {
        if (string.IsNullOrEmpty(sortBy))
        {
            return input.OrderByDescending(g => g.CreatedAt); // Default sorting by "New"
        }

        return sortBy switch
        {
            SortingOptions.MostCommented => input.OrderByDescending(g => g.CountComments()),
            SortingOptions.MostPopular => input.OrderByDescending(g => g.ViewCount),
            SortingOptions.PriceAsc => input.OrderBy(g => g.Price),
            SortingOptions.PriceDesc => input.OrderByDescending(g => g.Price),
            _ => input.OrderByDescending(g => g.CreatedAt), // Default to "New"
        };
    }
}
