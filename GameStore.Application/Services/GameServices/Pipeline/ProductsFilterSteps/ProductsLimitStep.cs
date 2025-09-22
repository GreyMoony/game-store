using GameStore.Domain.Entities.Mongo;

namespace GameStore.Application.Services.GameServices.Pipeline.ProductsFilterSteps;
public class ProductsLimitStep(int pageNumber, string? pageSize) : IPipelineStep<Product>
{
    public int TotalNumber { get; private set; }

    public IQueryable<Product> Process(IQueryable<Product> input)
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
