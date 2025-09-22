using GameStore.Application.DTOs.GameDtos;
using GameStore.Domain.Entities;
using Microsoft.Extensions.Caching.Memory;

namespace GameStore.Application.Services.GameServices.Pipeline;
public class CachingStep(IMemoryCache cache, string cacheKey, int totalCount) : IPipelineStep<ICommonGame>
{
    public IQueryable<ICommonGame> Process(IQueryable<ICommonGame> input)
    {
        var evaluatedGames = input.ToList();
        var toCache = new CachedCommonGames
        {
            Games = evaluatedGames,
            TotalFilteredCount = totalCount,
        };
        cache.Set(cacheKey, toCache, new MemoryCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10),
            Size = evaluatedGames.Count,
        });
        return input;
    }
}
