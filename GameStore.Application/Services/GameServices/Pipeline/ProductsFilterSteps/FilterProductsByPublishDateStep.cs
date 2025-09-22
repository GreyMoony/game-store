using GameStore.Domain.Entities.Mongo;

namespace GameStore.Application.Services.GameServices.Pipeline.ProductsFilterSteps;
public class FilterProductsByPublishDateStep(string publishDate) : IPipelineStep<Product>
{
    public IQueryable<Product> Process(IQueryable<Product> input)
    {
        return !string.IsNullOrEmpty(publishDate) ? Enumerable.Empty<Product>().AsQueryable() : input;
    }
}
