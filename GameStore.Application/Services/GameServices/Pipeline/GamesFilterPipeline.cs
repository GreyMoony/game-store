using GameStore.Domain.Entities;

namespace GameStore.Application.Services.GameServices.Pipeline;
#pragma warning disable IDE0028 // Simplify collection initialization
public class GamesFilterPipeline : IPipeline<Game>
{
    private readonly List<IPipelineStep<Game>> _steps = new();

    public IPipeline<Game> AddStep(IPipelineStep<Game> step)
    {
        _steps.Add(step);
        return this;
    }

    public IQueryable<Game> Execute(IQueryable<Game> input)
    {
        IQueryable<Game> output = input;
        foreach (var step in _steps)
        {
            output = step.Process(output);
        }

        return output;
    }
}
#pragma warning restore IDE0028 // Simplify collection initialization
