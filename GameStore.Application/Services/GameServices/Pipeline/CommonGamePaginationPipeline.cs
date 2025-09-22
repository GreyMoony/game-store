using GameStore.Domain.Entities;

namespace GameStore.Application.Services.GameServices.Pipeline;
#pragma warning disable IDE0028 // Simplify collection initialization
public class CommonGamePaginationPipeline : IPipeline<ICommonGame>
{
    private readonly List<IPipelineStep<ICommonGame>> _steps = new();

    public IPipeline<ICommonGame> AddStep(IPipelineStep<ICommonGame> step)
    {
        _steps.Add(step);
        return this;
    }

    public IQueryable<ICommonGame> Execute(IQueryable<ICommonGame> input)
    {
        IQueryable<ICommonGame> output = input;
        foreach (var step in _steps)
        {
            output = step.Process(output);
        }

        return output;
    }
}
#pragma warning restore IDE0028 // Simplify collection initialization
