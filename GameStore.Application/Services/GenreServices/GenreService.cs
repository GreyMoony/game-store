using AutoMapper;
using GameStore.Application.DTOs.GenreDtos;
using GameStore.Application.Helpers;
using GameStore.DAL.Interfaces;
using GameStore.Domain.Entities;
using GameStore.Domain.Entities.Mongo;
using GameStore.Domain.Exceptions;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace GameStore.Application.Services.GenreServices;
#pragma warning disable SA1010 // Opening square brackets should be spaced correctly
public class GenreService(
    IUnitOfWork unitOfWork,
    ICategoryRepository categoryRepository,
    IMapper mapper,
    EntityChangeLogger changeLogger) : IGenreService
{
    public async Task<GenreDto> AddGenreAsync(AddGenreDto genreDto)
    {
        ValidateGenreDto(genreDto);

        var parentGenreId = await ProcessParentGenreId(genreDto.ParentGenreId);
        await ValidateParentGenreIdAsync(parentGenreId);

        var genre = mapper.Map<Genre>(genreDto);
        genre.ParentGenreId = parentGenreId;

        await unitOfWork.Genres.AddAsync(genre);
        await unitOfWork.SaveAsync();

        changeLogger.LogEntityCreation(genre.GetType().Name, genre);
        return mapper.Map<GenreDto>(genre);
    }

    public async Task DeleteGenreAsync(string id)
    {
        if (Guid.TryParse(id, out var guidId))
        {
            await DeleteGenreAsync(guidId);
        }
        else if (int.TryParse(id, out var intId))
        {
            await DeleteCategoryAsync(intId);
        }
        else
        {
            // Invalid ID format
            throw new ArgumentException("The provided ID is not a valid Guid or int.", nameof(id));
        }
    }

    public async Task<IEnumerable<ShortGenreDto>> GetAllGenresAsync(bool includeDeleted = false)
    {
        var genres = mapper.Map<IEnumerable<ShortGenreDto>>(await unitOfWork.Genres.GetAllAsync(includeDeleted));
        var categories = mapper.Map<IEnumerable<ShortGenreDto>>(await categoryRepository.GetAllAsync(includeDeleted));

        var unitedResult = genres.Concat(categories);

        return unitedResult;
    }

    public async Task<GenreDto> GetGenreByIdAsync(string id, bool includeDeleted = false)
    {
        if (Guid.TryParse(id, out var guidId))
        {
            var genre = await unitOfWork.Genres.GetByIdAsync(guidId, includeDeleted)
            ?? throw new EntityNotFoundException($"Genre with Id {id} not found.");
            return mapper.Map<GenreDto>(genre);
        }
        else if (int.TryParse(id, out var intId))
        {
            var category = await categoryRepository.GetByIdAsync(intId, includeDeleted)
                ?? throw new EntityNotFoundException($"Category with Id {id} not found.");
            return mapper.Map<GenreDto>(category);
        }
        else
        {
            // Invalid ID format
            throw new ArgumentException("The provided ID is not a valid Guid or int.", nameof(id));
        }
    }

    public async Task<IEnumerable<ShortGenreDto>> GetGenresByGameKeyAsync(string key, bool includeDeleted = false)
    {
        var genres = await unitOfWork.Genres.GetByGameKeyAsync(key, includeDeleted);

        if (genres.IsNullOrEmpty())
        {
            var category = await categoryRepository.GetByProductKeyAsync(key, includeDeleted);
            if (category is null)
            {
                return [];
            }
            else
            {
                List<Category> categoryList = [category];
                return mapper.Map<IEnumerable<ShortGenreDto>>(categoryList);
            }
        }
        else
        {
            return mapper.Map<IEnumerable<ShortGenreDto>>(genres);
        }
    }

    public async Task<IEnumerable<ShortGenreDto>> GetGenresByParentIdAsync(string id, bool includeDeleted = false)
    {
        if (Guid.TryParse(id, out var guidId))
        {
            if (await unitOfWork.Genres.GetByIdAsync(guidId, includeDeleted) is null)
            {
                throw new EntityNotFoundException($"There is no genre with Id {id}");
            }

            var genres = await unitOfWork.Genres.GetByParentIdAsync(guidId, includeDeleted);

            return mapper.Map<IEnumerable<ShortGenreDto>>(genres);
        }
        else
        {
            return [];
        }
    }

    public async Task UpdateGenreAsync(GenreDto genreDto, bool includeDeleted = false)
    {
        if (Guid.TryParse(genreDto.Id, out var guidId))
        {
            await UpdateGameStoreGenreAsync(guidId, genreDto, includeDeleted);
        }
        else if (int.TryParse(genreDto.Id, out var intId))
        {
            await UpdateCategoryAsync(intId, genreDto, includeDeleted);
        }
        else
        {
            // Invalid ID format
            throw new ArgumentException("The provided ID of genreDto is not a valid Guid or int.", nameof(genreDto));
        }
    }

    private void ValidateGenreDto(AddGenreDto genreDto)
    {
        if (unitOfWork.Genres.NameExist(genreDto.Name))
        {
            throw new UniquePropertyException($"Genre with name {genreDto.Name} already exists.");
        }
    }

    private async Task ValidateParentGenreIdAsync(Guid? id)
    {
        if (id.HasValue && (await unitOfWork.Genres.GetByIdAsync(id.Value, true)) is null)
        {
            throw new IdsNotValidException($"There is no genre with Id {id.Value}");
        }
    }

    private async Task CheckCyclicReference(Guid genreId, Guid? parentGenreId)
    {
        var currentParentId = parentGenreId;

        while (currentParentId != null)
        {
            if (currentParentId == genreId)
            {
                throw new IdsNotValidException($"Provided id {parentGenreId} create cyclic reference problem.");
            }

            var parentGenre = await unitOfWork.Genres.GetByIdAsync(currentParentId.Value, true);

            if (parentGenre is null)
            {
                break;
            }

            currentParentId = parentGenre.ParentGenreId;
        }
    }

    private async Task DeleteGenreAsync(Guid id)
    {
        var genre = await unitOfWork.Genres.GetByIdAsync(id)
               ?? throw new EntityNotFoundException($"Genre with Id {id} not found.");

        foreach (var child in genre.SubGenres)
        {
            child.ParentGenreId = null;
        }

        unitOfWork.Genres.Delete(genre);
        changeLogger.LogEntityDeletion(genre.GetType().Name, genre);
        await unitOfWork.SaveAsync();
    }

    private async Task DeleteCategoryAsync(int id)
    {
        var category = await categoryRepository.GetByIdAsync(id) ??
            throw new EntityNotFoundException($"Category with Id {id} not found.");

        await categoryRepository.DeleteByIdAsync(id);
        changeLogger.LogEntityDeletion(category.GetType().Name, category);
    }

    private async Task UpdateGameStoreGenreAsync(Guid guidId, GenreDto genreDto, bool includeDeleted = false)
    {
        var genre = (await unitOfWork.Genres.GetByIdAsync(guidId, includeDeleted))
            ?? throw new EntityNotFoundException($"There is no genre with Id {genreDto.Id}");

        if (unitOfWork.Genres.NameExist(genreDto.Name)
            && genre.Name != genreDto.Name)
        {
            throw new UniquePropertyException($"Genre with name {genreDto.Name} already exist");
        }

        var parentGenreId = await ProcessParentGenreId(genreDto.ParentGenreId);
        await ValidateParentGenreIdAsync(parentGenreId);
        await CheckCyclicReference(guidId, parentGenreId);

        var oldGenre = JsonConvert.DeserializeObject<Genre>(
            JsonConvert.SerializeObject(genre, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            }));

        genre.Name = genreDto.Name;
        genre.ParentGenreId = parentGenreId;

        unitOfWork.Genres.Update(genre);
        await unitOfWork.SaveAsync();
        changeLogger.LogEntityChange("UPDATE", genre.GetType().Name, oldGenre, genre);
    }

    private async Task UpdateCategoryAsync(int id, GenreDto genreDto, bool includeDeleted = false)
    {
        var oldCategory = (await categoryRepository.GetByIdAsync(id, includeDeleted))
                ?? throw new EntityNotFoundException($"There is no category with Id {genreDto.Id}");

        if (oldCategory.CopiedToSql.HasValue && oldCategory.CopiedToSql.Value)
        {
            var genre = await unitOfWork.Genres.GetByCategoryIdAsync(oldCategory.CategoryID, includeDeleted)
                ?? throw new EntityNotFoundException($"There is no genre with CategoryID {oldCategory.CategoryID}");
            await UpdateGameStoreGenreAsync(genre.Id, genreDto, includeDeleted);
        }
        else
        {
            if (unitOfWork.Genres.NameExist(genreDto.Name))
            {
                throw new UniquePropertyException($"Genre with name {genreDto.Name} already exist");
            }

            var genre = mapper.Map<Genre>(oldCategory);
            genre.Name = genreDto.Name;

            var parentGenreId = await ProcessParentGenreId(genreDto.ParentGenreId);
            await ValidateParentGenreIdAsync(parentGenreId);
            genre.ParentGenreId = parentGenreId;

            await unitOfWork.Genres.AddAsync(genre);
            await unitOfWork.SaveAsync();
            await categoryRepository.MarkAsCopiedAsync(id);
            changeLogger.LogNorthwindEntityChange(
                "UPDATE", oldCategory.GetType().Name, oldCategory, genre);
        }
    }

    private async Task<Guid?> ProcessParentGenreId(string? id)
    {
        if (id.IsNullOrEmpty())
        {
            return null;
        }
        else if (Guid.TryParse(id, out var guidId))
        {
            return guidId;
        }
        else if (int.TryParse(id, out var intId))
        {
            var category = await categoryRepository.GetByIdAsync(intId, true)
                ?? throw new EntityNotFoundException($"There is no category with Id {intId}");

            var genre = mapper.Map<Genre>(category);
            await unitOfWork.Genres.AddAsync(genre);
            await unitOfWork.SaveAsync();

            await categoryRepository.MarkAsCopiedAsync(intId);

            changeLogger.LogNorthwindEntityChange(
                "COPY", category.GetType().Name, category, genre);

            return genre.Id;
        }
        else
        {
            // Invalid ID format
            throw new ArgumentException("The provided parentGenreId is not a valid Guid or int.", nameof(id));
        }
    }
}
#pragma warning restore SA1010 // Opening square brackets should be spaced correctly

