using GameStore.Domain.Entities.Mongo;

namespace GameStore.Application.Services.GameServices.Pipeline.ProductsFilterSteps;
public class ProductsPreSortingStep(string? sortBy) : IPipelineStep<Product>
{
    public IQueryable<Product> Process(IQueryable<Product> input)
    {
        return string.IsNullOrEmpty(sortBy)
            ? input
            : sortBy switch
            {
                SortingOptions.MostCommented => input,
                SortingOptions.MostPopular => input.OrderByDescending(g => g.ViewCount),
                SortingOptions.PriceAsc => input.OrderBy(g => g.Price),
                SortingOptions.PriceDesc => input.OrderByDescending(g => g.Price),
                _ => input,
            };
    }
}
