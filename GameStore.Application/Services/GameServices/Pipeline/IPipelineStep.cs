namespace GameStore.Application.Services.GameServices.Pipeline;
public interface IPipelineStep<T>
{
    IQueryable<T> Process(IQueryable<T> input);
}
