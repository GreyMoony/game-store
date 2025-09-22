using System.Text;
using AutoMapper;
using GameStore.Application.DTOs.GameDtos;
using GameStore.Application.Helpers;
using GameStore.Application.Services.BlobStorageService;
using GameStore.Application.Services.GameServices.Pipeline;
using GameStore.Application.Services.GameServices.Pipeline.GamesFilterSteps;
using GameStore.Application.Services.GameServices.Pipeline.ProductsFilterSteps;
using GameStore.DAL.Interfaces;
using GameStore.Domain.Entities;
using GameStore.Domain.Entities.Mongo;
using GameStore.Domain.Exceptions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace GameStore.Application.Services.GameServices;
#pragma warning disable SA1010, IDE0305 // Opening square brackets should be spaced correctly
public class GameService(
    IUnitOfWork unitOfWork,
    IUnitOfWorkMongo unitOfWorkNorthwind,
    IBlobStorageService blobStorageService,
    IMapper mapper,
    IMemoryCache cache,
    ILogger<GameService> logger,
    EntityChangeLogger changeLogger) : IGameService
{
    private const string DefaultImage = "default-game-image.png";

    public async Task<GameDto> AddGameAsync(AddGameDto game)
    {
        logger.LogInformation("Entering AddGameAsync method with AddGameDto: {AddGameDto}", game.ToString());
        var genreIds = await ProcessGenreCategoryIdsAsync(game.Genres);
        var publisherId = await ProcessPublisherSupplierIdAsync(game.Publisher);
        ValidateIds(genreIds, game.Platforms, publisherId);

        string gameKey;

        // If the key is manually provided, validate its uniqueness
        if (!string.IsNullOrEmpty(game.Game.Key))
        {
            gameKey = game.Game.Key;

            if ((await unitOfWork.Games.GetByKeyAsync(gameKey, true)) != null)
            {
                logger.LogWarning("Game key {Key} already exist", gameKey);
                throw new KeyIsNotValidException($"The manually entered key '{gameKey}' already exists.");
            }
        }
        else
        {
            // If no key is provided, generate one automatically and ensure it's unique
            logger.LogInformation("Generating new game key");
            gameKey = await GenerateUniqueGameKeyAsync(game.Game.Name);
        }

        var imageName = await blobStorageService.UploadImageAsync(game.Image, gameKey);

        var gameEntity = mapper.Map<Game>(game);
        gameEntity.Key = gameKey;
        gameEntity.CreatedAt = DateTime.UtcNow;
        gameEntity.GameGenres = genreIds.Select(genreId =>
            new GameGenre
            {
                GameId = gameEntity.Id,
                GenreId = genreId,
            }).ToList();
        gameEntity.PublisherId = publisherId;
        gameEntity.ImageName = imageName;

        logger.LogInformation("Mapped Game entity: {Game} will be added to database", gameEntity.ToString());

        await unitOfWork.Games.AddAsync(gameEntity);
        await unitOfWork.SaveAsync();

        changeLogger.LogEntityCreation(gameEntity.GetType().Name, gameEntity);
        return mapper.Map<GameDto>(gameEntity);
    }

    public async Task DeleteGameAsync(string key)
    {
        var game = await unitOfWork.Games.GetByKeyAsync(key);

        if (game is not null)
        {
            logger.LogInformation("Game: {Game} will be deleted", game.ToString());

            if (!string.IsNullOrEmpty(game.ImageName))
            {
                await blobStorageService.DeleteImageAsync(game.ImageName);
            }

            unitOfWork.Games.Delete(game);
            await unitOfWork.Games.DeleteTranslationsAsync(game.Id);
            changeLogger.LogEntityDeletion(game.GetType().Name, game);
            await unitOfWork.SaveAsync();
        }
        else
        {
            var product = await unitOfWorkNorthwind.Products.GetByKeyAsync(key) ??
                throw new EntityNotFoundException($"Game with Key {key} not found.");
            await unitOfWorkNorthwind.Products.DeleteByKeyAsync(key);
            changeLogger.LogEntityDeletion(product.GetType().Name, product);
        }
    }

    public async Task<IEnumerable<GameDto>> GetAllGamesAsync(bool includeDeleted = false)
    {
        var games = mapper.Map<IEnumerable<GameDto>>(await unitOfWork.Games.GetAllAsync(includeDeleted));
        var products = mapper.Map<IEnumerable<GameDto>>(await unitOfWorkNorthwind.Products.GetAllAsync(includeDeleted));

        var unitedResult = games.Concat(products);

        return unitedResult;
    }

    public PagedGames GetGames(GameQuery query, bool includeDeleted = false)
    {
        var cacheKey = GenerateCacheKey(query);
        var gamesPipeline = new GamesFilterPipeline();
        var productsPipeline = new ProductsFilterPipeline();
        var paginationPipeline = new CommonGamePaginationPipeline();
        int totalItems;

        IQueryable<Game> games;
        IQueryable<Product> products;
        IQueryable<ICommonGame> unifiedEntities;
        if (query.Trigger != Triggers.ApplyFilters
            && cache.TryGetValue(cacheKey, out CachedCommonGames? cachedCommonGames)
            && cachedCommonGames != null)
        {
            unifiedEntities = cachedCommonGames.Games.AsQueryable(); // Use cached filtered results
            totalItems = cachedCommonGames.TotalFilteredCount;
        }
        else
        {
            games = unitOfWork.Games.GetGamesQuery(includeDeleted);
            products = unitOfWorkNorthwind.Products.GetQuery(includeDeleted);

            var gamesLimitStep = new LimitStep(query.Page, query.PageCount);
            var productsLimitStep = new ProductsLimitStep(query.Page, query.PageCount);
            AddGameFiltersSteps(gamesPipeline, query, gamesLimitStep);
            AddProductFiltersSteps(productsPipeline, query, productsLimitStep);

            var filteredGames = gamesPipeline.Execute(games).ToList().Cast<ICommonGame>();
            var filteredProducts = productsPipeline.Execute(products).ToList().Cast<ICommonGame>();
            unifiedEntities = filteredGames.Concat(filteredProducts).AsQueryable();

            totalItems = gamesLimitStep.TotalNumber + productsLimitStep.TotalNumber; // Count total items after filtering

            paginationPipeline.AddStep(new CachingStep(cache, cacheKey, totalItems));
        }

        paginationPipeline.AddStep(new SortingStep(query.Sort));

        var paginationStep = new PaginationStep(query.Page, query.PageCount);
        paginationPipeline.AddStep(paginationStep);

        var processedGames = paginationPipeline.Execute(unifiedEntities).ToList();

        double pageCount = double.TryParse(query.PageCount, out double count) ?
            count :
            totalItems;

        return new PagedGames
        {
            Games = mapper.Map<IEnumerable<GameDto>>(processedGames),
            TotalPages = (int)Math.Ceiling(totalItems / pageCount),
            CurrentPage = query.Page,
        };
    }

    public async Task<GameDto> GetGameByIdAsync(string id, bool includeDeleted = false)
    {
        if (Guid.TryParse(id, out var guidId))
        {
            var game = await unitOfWork.Games.GetByIdAsync(guidId, includeDeleted)
            ?? throw new EntityNotFoundException($"Game with Id {id} not found.");

            logger.LogInformation("Game: {Game} was found by id: {Id}", game.ToString(), id);
            return mapper.Map<GameDto>(game);
        }
        else if (int.TryParse(id, out var intId))
        {
            var product = await unitOfWorkNorthwind.Products.GetByIdAsync(intId, includeDeleted)
                ?? throw new EntityNotFoundException($"Game with Id {id} not found.");
            return mapper.Map<GameDto>(product);
        }
        else
        {
            // Invalid ID format
            throw new ArgumentException("The provided ID is not a valid Guid or int.", nameof(id));
        }
    }

    public async Task<GameDto> GetGameByKeyAsync(string key, bool includeDeleted = false)
    {
        var game = await unitOfWork.Games.GetByKeyAsync(key, includeDeleted);
        if (game is null)
        {
            var product = await unitOfWorkNorthwind.Products.GetByKeyAsync(key, includeDeleted)
                ?? throw new EntityNotFoundException($"Game with Key {key} not found.");
            return mapper.Map<GameDto>(product);
        }
        else
        {
            logger.LogInformation("Game: {Game} was found by key: {Key}", game.ToString(), key);
            return mapper.Map<GameDto>(game);
        }
    }

    public async Task<IEnumerable<GameDto>> GetGamesByGenreAsync(string genreId, bool includeDeleted = false)
    {
        if (Guid.TryParse(genreId, out var guidId))
        {
            return mapper.Map<IEnumerable<GameDto>>(await unitOfWork.Games.GetByGenreAsync(guidId, includeDeleted));
        }
        else if (int.TryParse(genreId, out var intId))
        {
            return mapper.Map<IEnumerable<GameDto>>(await unitOfWorkNorthwind.Products.GetByCategoryAsync(intId, includeDeleted));
        }
        else
        {
            // Invalid ID format
            throw new ArgumentException("The provided ID is not a valid Guid or int.", nameof(genreId));
        }
    }

    public async Task<IEnumerable<GameDto>> GetGamesByPlatformAsync(Guid platformId, bool includeDeleted = false)
    {
        var games = mapper.Map<IEnumerable<GameDto>>(await unitOfWork.Games.GetByPlatformAsync(platformId, includeDeleted));

        return games;
    }

    public async Task<IEnumerable<GameDto>> GetGamesByPublisherAsync(string companyName, bool includeDeleted = false)
    {
        var games = mapper.Map<IEnumerable<GameDto>>(
            await unitOfWork.Games.GetByPublisherAsync(companyName, includeDeleted));
        var products = mapper.Map<IEnumerable<GameDto>>(
            await unitOfWorkNorthwind.Products.GetBySupplierNameAsync(companyName, includeDeleted));

        var unitedResult = games.Concat(products);

        return unitedResult;
    }

    public async Task<Game> UpdateGameAsync(UpdateGameDto game, bool includeDeleted = false)
    {
        if (Guid.TryParse(game.Game.Id, out var guidId))
        {
            return await UpdateGameStoreGameAsync(guidId, game, includeDeleted);
        }
        else if (int.TryParse(game.Game.Id, out var intId))
        {
            return await UpdateProductAsync(intId, game, includeDeleted);
        }
        else
        {
            // Invalid ID format
            throw new ArgumentException("The provided game ID is not a valid Guid or int.", nameof(game));
        }
    }

    public async Task<int> GamesCount(bool includeDeleted = false)
    {
        var gamesCount = await unitOfWork.Games.CountAsync(includeDeleted);
        var productsCount = (await unitOfWorkNorthwind.Products.GetAllAsync(includeDeleted)).Count();
        return gamesCount + productsCount;
    }

    public async Task IncrementViewCount(string gameKey)
    {
        var game = await unitOfWork.Games.GetByKeyAsync(gameKey, true);
        if (game is not null)
        {
            game.ViewCount++;
            unitOfWork.Games.Update(game);
            await unitOfWork.SaveAsync();
        }
        else
        {
            var success = await unitOfWorkNorthwind.Products.IncrementViewCount(gameKey);
            if (!success)
            {
                throw new EntityNotFoundException($"Game with Key {gameKey} not found.");
            }
        }
    }

    public async Task<GameImageDto?> GetGameImageAsync(string gameKey)
    {
        var game = await unitOfWork.Games.GetByKeyAsync(gameKey);

        return (game is null || string.IsNullOrEmpty(game.ImageName)) ?
            await blobStorageService.GetImageAsync(DefaultImage) :
            await blobStorageService.GetImageAsync(game.ImageName);
    }

    public async Task<GameDto> GetLocalizedGameAsync(string key, string lang, bool includeDeleted = false)
    {
        var game = await unitOfWork.Games.GetLocalizedGameAsync(key, lang, includeDeleted);
        if (game is null)
        {
            var product = await unitOfWorkNorthwind.Products.GetByKeyAsync(key, includeDeleted)
                ?? throw new EntityNotFoundException($"Game with Key {key} not found.");
            return mapper.Map<GameDto>(product);
        }
        else
        {
            logger.LogInformation("Game: {Game} was found by key: {Key}", game.ToString(), key);
            return mapper.Map<GameDto>(game);
        }
    }

    // Returns list of genre ids that are not in database
    private List<Guid> InvalidGenreIds(IEnumerable<Guid> genreIds)
    {
        var invalidGenreIds = genreIds.Where(id => !unitOfWork.Genres.IdExist(id, true)).ToList();
        return invalidGenreIds;
    }

    // Returns list of platform ids that are not in database
    private List<Guid> InvalidPlatformIds(IEnumerable<Guid> platformIds)
    {
        var invalidPlatformIds = platformIds.Where(id => !unitOfWork.Platforms.IdExist(id, true)).ToList();
        return invalidPlatformIds;
    }

    // Throws exception if genreIds or platformIds contain invalid id
    private void ValidateIds(IEnumerable<Guid> genreIds, IEnumerable<Guid> platformIds, Guid publisherId)
    {
        logger.LogInformation("Validating provided ids");

        var invalidGenreIds = InvalidGenreIds(genreIds);
        if (invalidGenreIds.Count != 0)
        {
            logger.LogWarning("Invalid genre ids were provided");
            throw new IdsNotValidException($"Invalid genres IDs: {string.Join(", ", invalidGenreIds)}");
        }

        var invalidPlatformIds = InvalidPlatformIds(platformIds);
        if (invalidPlatformIds.Count != 0)
        {
            logger.LogWarning("Invalid platform ids were provided");
            throw new IdsNotValidException($"Invalid platforms IDs: {string.Join(", ", invalidPlatformIds)}");
        }

        if (!unitOfWork.Publishers.IdExist(publisherId))
        {
            logger.LogWarning("Invalid publisher id was provided");
            throw new IdsNotValidException($"Invalid publisher ID: {publisherId}");
        }

        logger.LogInformation("Provided ids are valid");
    }

    private async Task<string> GenerateUniqueGameKeyAsync(string gameName)
    {
        // Generate initial game key based on the game name
        string baseKey = KeyStringHelper.CreateKey(gameName);
        string uniqueKey = baseKey;
        int suffix = 1;

        // Check if the generated key already exists
        while ((await unitOfWork.Games.GetByKeyAsync(uniqueKey, true)) != null)
        {
            // Append a suffix and try again
            uniqueKey = $"{baseKey}-{suffix}";
            suffix++;
        }

        return uniqueKey;
    }

    private static string GenerateCacheKey(GameQuery query)
    {
        var keyBuilder = new StringBuilder();
        keyBuilder.Append($"Genres:{string.Join(",", query.Genres)}|");
        keyBuilder.Append($"Platforms:{string.Join(",", query.Platforms)}|");
        keyBuilder.Append($"Publishers:{string.Join(",", query.Publishers)}|");
        keyBuilder.Append($"MinPrice:{query.MinPrice}|");
        keyBuilder.Append($"MaxPrice:{query.MaxPrice}|");
        keyBuilder.Append($"Name:{query.Name}|");
        keyBuilder.Append($"DatePublishing:{query.DatePublishing}|");
        keyBuilder.Append($"Sort:{query.Sort}|");
        keyBuilder.Append($"Page:{query.Page}|");
        keyBuilder.Append($"PageCount:{query.PageCount}|");
        return keyBuilder.ToString();
    }

    private static void AddGameFiltersSteps(
        GamesFilterPipeline pipeline, GameQuery query, LimitStep limitStep)
    {
        if (query.Genres.Count != 0)
        {
            pipeline.AddStep(new FilterByGenreStep(query.Genres));
        }

        if (query.Platforms.Count != 0)
        {
            pipeline.AddStep(new FilterByPlatformsStep(query.Platforms));
        }

        if (query.Publishers.Count != 0)
        {
            pipeline.AddStep(new FilterByPublishersStep(query.Publishers));
        }

        if (query.MinPrice.HasValue)
        {
            pipeline.AddStep(new FilterByMinPriceStep(query.MinPrice.Value));
        }

        if (query.MaxPrice.HasValue)
        {
            pipeline.AddStep(new FilterByMaxPriceStep(query.MaxPrice.Value));
        }

        if (!string.IsNullOrEmpty(query.Name) && query.Name.Length >= 3)
        {
            pipeline.AddStep(new FilterByNameStep(query.Name));
        }

        if (!string.IsNullOrEmpty(query.DatePublishing))
        {
            pipeline.AddStep(new FilterByPublishDateStep(query.DatePublishing));
        }

        pipeline.AddStep(new PreSortingStep(query.Sort));
        pipeline.AddStep(limitStep);
    }

    private static void AddProductFiltersSteps(
        ProductsFilterPipeline pipeline, GameQuery query, ProductsLimitStep limitStep)
    {
        if (query.Genres.Count != 0)
        {
            pipeline.AddStep(new FilterByCategoryStep(query.Genres));
        }

        if (query.Platforms.Count != 0)
        {
            pipeline.AddStep(new FilterProductsByPlatformsStep(query.Platforms));
        }

        if (query.Publishers.Count != 0)
        {
            pipeline.AddStep(new FilterProductsBySuppliersStep(query.Publishers));
        }

        if (query.MinPrice.HasValue)
        {
            pipeline.AddStep(new FilterProductsByMinPriceStep(query.MinPrice.Value));
        }

        if (query.MaxPrice.HasValue)
        {
            pipeline.AddStep(new FilterProductsByMaxPriceStep(query.MaxPrice.Value));
        }

        if (!string.IsNullOrEmpty(query.Name) && query.Name.Length >= 3)
        {
            pipeline.AddStep(new FilterProductsByNameStep(query.Name));
        }

        if (!string.IsNullOrEmpty(query.DatePublishing))
        {
            pipeline.AddStep(new FilterProductsByPublishDateStep(query.DatePublishing));
        }

        pipeline.AddStep(new ProductsPreSortingStep(query.Sort));
        pipeline.AddStep(limitStep);
    }

    private async Task<Game> UpdateGameStoreGameAsync(Guid id, UpdateGameDto game, bool includeDeleted)
    {
        var gameEntity = await GetGame(id, includeDeleted);

        if (!includeDeleted && gameEntity is null)
        {
            var deletedGame = await unitOfWork.Games.GetByIdAsync(id, true);
            if (deletedGame is null)
            {
                throw new EntityNotFoundException($"Game with Id {id} not found.");
            }
            else
            {
                throw new UnauthenticatedException($"Game with Id {id} not found.");
            }
        }
        else if (includeDeleted && gameEntity is null)
        {
            throw new EntityNotFoundException($"Game with Id {id} not found.");
        }

        logger.LogInformation("Game: {Game} was found by id: {Id}", gameEntity.ToString(), game.Game.Id);

        var genreIds = await ProcessGenreCategoryIdsAsync(game.Genres);
        var publisherId = await ProcessPublisherSupplierIdAsync(game.Publisher);

        ValidateIds(genreIds, game.Platforms, publisherId);
        if ((await unitOfWork.Games.GetByKeyAsync(game.Game.Key, true)) != null && game.Game.Key != gameEntity.Key)
        {
            throw new KeyIsNotValidException($"The entered key '{game.Game.Key}' already exists.");
        }

        var oldGame = JsonConvert.DeserializeObject<Game>(
            JsonConvert.SerializeObject(gameEntity, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            }));

        gameEntity.Name = game.Game.Name;
        gameEntity.Key = game.Game.Key.IsNullOrEmpty() ? gameEntity.Key : game.Game.Key;
        gameEntity.Description = game.Game.Description;
        gameEntity.Price = game.Game.Price;
        gameEntity.Discount = game.Game.Discount;
        gameEntity.UnitInStock = game.Game.UnitInStock;
        gameEntity.PublisherId = publisherId;

        if (!string.IsNullOrEmpty(game.Image))
        {
            var imageName = await blobStorageService.UploadImageAsync(game.Image, gameEntity.Key);
            gameEntity.ImageName = imageName;
        }

        gameEntity.GameGenres.Clear();
        gameEntity.GamePlatforms.Clear();

        gameEntity.GameGenres = genreIds.Select(genreId => new GameGenre
        {
            GameId = gameEntity.Id,
            GenreId = genreId,
        }).ToList();
        gameEntity.GamePlatforms = game.Platforms.Select(platformId => new GamePlatform
        {
            PlatformId = platformId,
            GameId = gameEntity.Id,
        }).ToList();

        logger.LogInformation("Game: {Game} is going to be updated", gameEntity.ToString());
        unitOfWork.Games.Update(gameEntity);
        await unitOfWork.SaveAsync();
        changeLogger.LogEntityChange("UPDATE", gameEntity.GetType().Name, oldGame, gameEntity);
        return gameEntity;
    }

    private async Task<Game> UpdateProductAsync(int id, UpdateGameDto game, bool includeDeleted)
    {
        var oldProduct = await GetProduct(id, includeDeleted);

        if (oldProduct.CopiedToSql.HasValue && oldProduct.CopiedToSql.Value)
        {
            var copiedGame = await unitOfWork.Games.GetByProductIdAsync(oldProduct.ProductID!.Value);
            return await UpdateGameStoreGameAsync(copiedGame.Id, game, includeDeleted);
        }
        else
        {
            var genreIds = await ProcessGenreCategoryIdsAsync(game.Genres);
            var publisherId = await ProcessPublisherSupplierIdAsync(game.Publisher);
            ValidateIds(genreIds, game.Platforms, publisherId);
            if ((await unitOfWork.Games.GetByKeyAsync(game.Game.Key, true)) != null)
            {
                throw new KeyIsNotValidException($"The entered key '{game.Game.Key}' already exists.");
            }

            var newGame = mapper.Map<Game>(oldProduct);
            newGame.Name = game.Game.Name;
            newGame.Key = game.Game.Key.IsNullOrEmpty() ? newGame.Key : game.Game.Key;
            newGame.Description = game.Game.Description;
            newGame.Price = game.Game.Price;
            newGame.Discount = game.Game.Discount;
            newGame.UnitInStock = game.Game.UnitInStock;

            if (!string.IsNullOrEmpty(game.Image))
            {
                var imageName = await blobStorageService.UploadImageAsync(game.Image, newGame.Key);
                newGame.ImageName = imageName;
            }

            newGame.PublisherId = publisherId;
            newGame.GameGenres = genreIds.Select(genreId => new GameGenre
            {
                GameId = newGame.Id,
                GenreId = genreId,
            }).ToList();
            newGame.GamePlatforms = game.Platforms.Select(platformId => new GamePlatform
            {
                PlatformId = platformId,
                GameId = newGame.Id,
            }).ToList();

            await unitOfWork.Games.AddAsync(newGame);
            await unitOfWork.SaveAsync();

            await unitOfWorkNorthwind.Products.MarkAsCopiedAsync(id);
            changeLogger.LogNorthwindEntityChange(
                "UPDATE", oldProduct.GetType().Name, oldProduct, newGame);
            return newGame;
        }
    }

    private async Task<Guid> ProcessPublisherSupplierIdAsync(string id)
    {
        if (Guid.TryParse(id, out var guidId))
        {
            return guidId;
        }
        else if (int.TryParse(id, out var intId))
        {
            return await CopySupplierToGameStoreAsync(intId);
        }
        else
        {
            // Invalid ID format
            throw new ArgumentException("The provided game ID is not a valid Guid or int.", nameof(id));
        }
    }

    private async Task<IEnumerable<Guid>> ProcessGenreCategoryIdsAsync(IEnumerable<string> ids)
    {
        List<Guid> genreIds = [];
        foreach (var id in ids)
        {
            if (Guid.TryParse(id, out var guidId))
            {
                genreIds.Add(guidId);
            }
            else if (int.TryParse(id, out var intId))
            {
                var newGuid = await CopyCategoryToGameStoreAsync(intId);

                genreIds.Add(newGuid);
            }
        }

        return genreIds;
    }

    private async Task<Guid> CopySupplierToGameStoreAsync(int id)
    {
        var supplier = await unitOfWorkNorthwind.Suppliers.GetByIdAsync(id, true);

        if (supplier.CopiedToSql.HasValue && supplier.CopiedToSql.Value)
        {
            var publisher = await unitOfWork.Publishers.GetBySupplierIdAsync(id, true);
            return publisher.Id;
        }
        else
        {
            var publisher = mapper.Map<Publisher>(supplier);

            await unitOfWork.Publishers.AddAsync(publisher);
            await unitOfWork.SaveAsync();
            changeLogger.LogNorthwindEntityChange(
                "COPY", supplier.GetType().Name, supplier, publisher);
            await unitOfWorkNorthwind.Suppliers.MarkAsCopiedAsync(id);

            return publisher.Id;
        }
    }

    private async Task<Guid> CopyCategoryToGameStoreAsync(int id)
    {
        var category = await unitOfWorkNorthwind.Categories.GetByIdAsync(id, true);

        if (category.CopiedToSql.HasValue && category.CopiedToSql.Value)
        {
            var genre = await unitOfWork.Genres.GetByCategoryIdAsync(id, true);
            return genre.Id;
        }
        else
        {
            var genre = mapper.Map<Genre>(category);

            await unitOfWork.Genres.AddAsync(genre);
            await unitOfWork.SaveAsync();
            changeLogger.LogNorthwindEntityChange("COPY", category.GetType().Name, category, genre);
            await unitOfWorkNorthwind.Categories.MarkAsCopiedAsync(id);

            return genre.Id;
        }
    }

    private async Task<Game> GetGame(Guid id, bool includeDeleted)
    {
        var gameEntity = await unitOfWork.Games.GetByIdAsync(id, includeDeleted);

        if (!includeDeleted && gameEntity is null)
        {
            var deletedGame = await unitOfWork.Games.GetByIdAsync(id, true);
            if (deletedGame is null)
            {
                throw new EntityNotFoundException($"Game with Id {id} not found.");
            }
            else
            {
                throw new UnauthenticatedException($"You have no rights to manage game with id {id}");
            }
        }
        else if (includeDeleted && gameEntity is null)
        {
            throw new EntityNotFoundException($"Game with Id {id} not found.");
        }

        return gameEntity;
    }

    private async Task<Product> GetProduct(int id, bool includeDeleted)
    {
        var product = await unitOfWorkNorthwind.Products.GetByIdAsync(id, includeDeleted);

        if (!includeDeleted && product is null)
        {
            var deletedProduct = await unitOfWorkNorthwind.Products.GetByIdAsync(id, true);
            if (deletedProduct is null)
            {
                throw new EntityNotFoundException($"Product with Id {id} not found.");
            }
            else
            {
                throw new UnauthenticatedException($"You have no rights to manage product with id {id}");
            }
        }
        else if (includeDeleted && product is null)
        {
            throw new EntityNotFoundException($"Product with Id {id} not found.");
        }

        return product;
    }
}
#pragma warning restore SA1010, IDE0305 // Opening square brackets should be spaced correctly
