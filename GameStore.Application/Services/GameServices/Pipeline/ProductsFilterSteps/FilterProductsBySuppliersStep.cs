using GameStore.Application.Helpers;
using GameStore.Domain.Entities.Mongo;

namespace GameStore.Application.Services.GameServices.Pipeline.ProductsFilterSteps;

public class FilterProductsBySuppliersStep(List<string> suppliers) : IPipelineStep<Product>
{
    public IQueryable<Product> Process(IQueryable<Product> input)
    {
        var parsedIntIds = IdParser.GetInts(suppliers);

        return input.Where(p => parsedIntIds.Contains(p.SupplierID!.Value));
    }
}