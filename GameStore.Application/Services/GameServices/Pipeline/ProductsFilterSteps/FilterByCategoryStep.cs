using GameStore.Application.Helpers;
using GameStore.Domain.Entities.Mongo;

namespace GameStore.Application.Services.GameServices.Pipeline.ProductsFilterSteps;
public class FilterByCategoryStep(List<string> categories) : IPipelineStep<Product>
{
    public IQueryable<Product> Process(IQueryable<Product> input)
    {
        var parsedIntIds = IdParser.GetInts(categories);

        return input
            .Where(product => parsedIntIds.Contains(product.CategoryID!.Value));
    }
}
