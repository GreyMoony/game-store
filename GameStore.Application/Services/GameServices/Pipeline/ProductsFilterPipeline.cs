using GameStore.Domain.Entities.Mongo;

namespace GameStore.Application.Services.GameServices.Pipeline;
#pragma warning disable IDE0028 // Simplify collection initialization
public class ProductsFilterPipeline : IPipeline<Product>
{
    private readonly List<IPipelineStep<Product>> _steps = new();

    public IPipeline<Product> AddStep(IPipelineStep<Product> step)
    {
        _steps.Add(step);
        return this;
    }

    public IQueryable<Product> Execute(IQueryable<Product> input)
    {
        IQueryable<Product> output = input;
        foreach (var step in _steps)
        {
            output = step.Process(output);
        }

        return output;
    }
}
#pragma warning restore IDE0028 // Simplify collection initialization
