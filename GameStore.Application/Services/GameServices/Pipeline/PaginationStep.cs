using GameStore.Domain.Entities;

namespace GameStore.Application.Services.GameServices.Pipeline;
public class PaginationStep(int pageNumber, string? pageSize) : IPipelineStep<ICommonGame>
{
    public IQueryable<ICommonGame> Process(IQueryable<ICommonGame> input)
    {
        if (pageNumber < 1)
        {
            pageNumber = 1;
        }

        return int.TryParse(pageSize, out int pageItemsCount)
            ? input
                .Skip((pageNumber - 1) * pageItemsCount)
                .Take(pageItemsCount)
            : input;
    }
}
