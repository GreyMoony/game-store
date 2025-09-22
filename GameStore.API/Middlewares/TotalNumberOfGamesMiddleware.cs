using GameStore.Application.Services.GameServices;
using GameStore.Domain.Constants;
using Microsoft.Extensions.Caching.Memory;

namespace GameStore.API.Middlewares;

public class TotalNumberOfGamesMiddleware(RequestDelegate next, IMemoryCache cache)
{
    public const string GamesCountHeader = "x-total-numbers-of-games";
    private const string GamesCountCacheKey = "TotalGamesCount";

    public async Task InvokeAsync(HttpContext context, IGameService gameService)
    {
        var user = context.User;
        var includeDeleted = user.Claims.Any(
            c => c.Type == Permissions.PermissionClaim &&
            c.Value == Permissions.ViewDeletedGames);

        // Use OnStarting to modify headers right before the response is sent
        context.Response.OnStarting(async () =>
        {
            // Check if we need to invalidate the cache based on the status code
            if (context.Response.StatusCode is StatusCodes.Status201Created or
                StatusCodes.Status204NoContent)
            {
                cache.Remove(GamesCountCacheKey);
            }

            // Try to get the count from cache
            if (!cache.TryGetValue(GamesCountCacheKey, out int totalGames))
            {
                // If not in cache, get it from the service
                totalGames = await gameService.GamesCount(includeDeleted);

                var cacheOptions = new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(1)) // Cache for 1 minute
                    .SetSize(1); // Cache for 1 minute

                cache.Set(GamesCountCacheKey, totalGames, cacheOptions);
            }

            // Add the header to the response
            context.Response.Headers[GamesCountHeader] = totalGames.ToString();
        });

        // Call the next middleware in the pipeline
        await next(context);
    }
}
