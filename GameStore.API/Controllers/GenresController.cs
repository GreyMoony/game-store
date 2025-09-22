using AutoMapper;
using GameStore.API.Models.GenreModels;
using GameStore.Application.DTOs.GenreDtos;
using GameStore.Application.Services.GenreServices;
using GameStore.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameStore.API.Controllers;
[Route("api/")]
[ApiController]
public class GenresController(IGenreService genreService, IMapper mapper) : ControllerBase
{
    private bool IncludeDeleted => User.Identity?.IsAuthenticated == true &&
                   User.Claims.Any(c => c.Type == Permissions.PermissionClaim &&
                                        c.Value == Permissions.ViewDeletedGames);

    [Authorize(Policy = Policies.CanManageEntities)]
    [HttpPost("genres")]
    public async Task<IActionResult> AddGenre([FromBody] AddGenreRequestModel genreRequestModel)
    {
        if (genreRequestModel == null)
        {
            return BadRequest("Genre data can not be null");
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var genreDto = mapper.Map<AddGenreDto>(genreRequestModel.Genre);

        var genre = await genreService.AddGenreAsync(genreDto);
        return CreatedAtAction(nameof(GetGenreById), new { id = genre.Id }, genre);
    }

    [HttpGet("genres/{id}")]
    public async Task<IActionResult> GetGenreById(string id)
    {
        var genre = mapper.Map<GenreModel>(
            await genreService.GetGenreByIdAsync(id, IncludeDeleted));

        return Ok(genre);
    }

    [HttpGet("genres")]
    public async Task<IActionResult> GetAllGenres()
    {
        var genres = mapper.Map<IEnumerable<ShortGenreModel>>(
            await genreService.GetAllGenresAsync(IncludeDeleted));
        return Ok(genres);
    }

    [HttpGet("games/{key}/genres")]
    public async Task<IActionResult> GetGenresByGameKey(string key)
    {
        var genres = mapper.Map<IEnumerable<ShortGenreModel>>(
            await genreService.GetGenresByGameKeyAsync(key, IncludeDeleted));

        return Ok(genres);
    }

    [HttpGet("genres/{id}/genres")]
    public async Task<IActionResult> GetGenresByParentId(string id)
    {
        var genres = mapper.Map<IEnumerable<ShortGenreModel>>(
            await genreService.GetGenresByParentIdAsync(id, IncludeDeleted));

        return Ok(genres);
    }

    [Authorize(Policy = Policies.CanManageEntities)]
    [HttpPut("genres")]
    public async Task<IActionResult> UpdateGenre([FromBody] UpdateGenreModel genreModel)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var genreDto = mapper.Map<GenreDto>(genreModel.Genre);

        await genreService.UpdateGenreAsync(genreDto);

        return Ok();
    }

    [Authorize(Policy = Policies.CanManageEntities)]
    [HttpDelete("genres/{id}")]
    public async Task<IActionResult> DeleteGenre(string id)
    {
        await genreService.DeleteGenreAsync(id);

        return NoContent();
    }
}
