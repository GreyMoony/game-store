using GameStore.Domain.Entities.Mongo;
using Microsoft.IdentityModel.Tokens;

namespace GameStore.Application.Services.GameServices.Pipeline.ProductsFilterSteps;
public class FilterProductsByPlatformsStep(List<string> platforms) : IPipelineStep<Product>
{
    public IQueryable<Product> Process(IQueryable<Product> input)
    {
        return !platforms.IsNullOrEmpty() ? Enumerable.Empty<Product>().AsQueryable() : input;
    }
}
