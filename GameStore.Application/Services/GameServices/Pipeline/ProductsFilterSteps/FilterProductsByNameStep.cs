using GameStore.Domain.Entities.Mongo;

namespace GameStore.Application.Services.GameServices.Pipeline.ProductsFilterSteps;
#pragma warning disable CA1862, CA1311
public class FilterProductsByNameStep(string name) : IPipelineStep<Product>
{
    public IQueryable<Product> Process(IQueryable<Product> input)
    {
        return input.Where(g => g.Name.ToLower().Contains(name.ToLower()));
    }
}
