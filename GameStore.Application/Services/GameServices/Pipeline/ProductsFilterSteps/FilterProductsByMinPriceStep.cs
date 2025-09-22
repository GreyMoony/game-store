using GameStore.Domain.Entities.Mongo;

namespace GameStore.Application.Services.GameServices.Pipeline.ProductsFilterSteps;
public class FilterProductsByMinPriceStep(double minPrice) : IPipelineStep<Product>
{
    public IQueryable<Product> Process(IQueryable<Product> input)
    {
        return input.Where(g => g.Price >= minPrice);
    }
}
