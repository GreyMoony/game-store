using GameStore.Domain.Entities.Mongo;

namespace GameStore.Application.Services.GameServices.Pipeline.ProductsFilterSteps;
public class FilterProductsByMaxPriceStep(double maxPrice) : IPipelineStep<Product>
{
    public IQueryable<Product> Process(IQueryable<Product> input)
    {
        return input.Where(g => g.Price <= maxPrice);
    }
}
