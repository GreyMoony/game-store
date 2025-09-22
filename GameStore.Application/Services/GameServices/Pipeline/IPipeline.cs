namespace GameStore.Application.Services.GameServices.Pipeline;
public interface IPipeline<T>
{
    IPipeline<T> AddStep(IPipelineStep<T> step);

    IQueryable<T> Execute(IQueryable<T> input);
}
