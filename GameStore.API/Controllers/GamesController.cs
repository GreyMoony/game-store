using AutoMapper;
using GameStore.API.Helpers.RequestContext;
using GameStore.API.Models.GameModels;
using GameStore.Application.DTOs.GameDtos;
using GameStore.Application.Services.GameServices;
using GameStore.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace GameStore.API.Controllers;
[Route("api/")]
[ApiController]
public class GamesController(
    IGameService gameService,
    IMapper mapper,
    IRequestLocalizationContext loc) : ControllerBase
{
    private bool IncludeDeleted => User.Identity?.IsAuthenticated == true &&
                   User.Claims.Any(c => c.Type == Permissions.PermissionClaim &&
                                        c.Value == Permissions.ViewDeletedGames);

    private bool CanEditDeleted => User.Identity?.IsAuthenticated == true &&
                   User.Claims.Any(c => c.Type == Permissions.PermissionClaim &&
                                        c.Value == Permissions.EditDeletedGames);

    [HttpGet("games/all")]
    public async Task<IActionResult> GetAllGamesAsync()
    {
        var games = await gameService.GetAllGamesAsync(IncludeDeleted);
        return Ok(games);
    }

    [HttpGet("games")]
    public IActionResult GetGames([FromQuery] GameQueryModel gameQuery)
    {
        if (string.IsNullOrEmpty(gameQuery.PageCount))
        {
            gameQuery.PageCount = PaginationOptions.Ten;
        }

        var query = mapper.Map<GameQuery>(gameQuery);
        var games = gameService.GetGames(query, IncludeDeleted);
        return Ok(games);
    }

    [Authorize(Policy = Policies.CanManageEntities)]
    [HttpPost("games")]
    public async Task<IActionResult> AddGame([FromBody] AddGameModel gameModel)
    {
        if (gameModel == null)
        {
            return BadRequest("Game data cannot be null.");
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var addGameDto = mapper.Map<AddGameDto>(gameModel);

        var game = await gameService.AddGameAsync(addGameDto);

        return CreatedAtAction(nameof(GetGameByKey), new { key = game.Key }, game);
    }

    [HttpGet("games/{key}")]
    public async Task<IActionResult> GetGameByKey(string key)
    {
        var lang = loc.CurrentLanguage;
        var game = mapper.Map<GameModel>(await gameService.GetLocalizedGameAsync(key, lang, IncludeDeleted));
        await gameService.IncrementViewCount(key);

        return Ok(game);
    }

    [HttpGet("games/find/{id}")]
    [ResponseCache(Duration = 60)]
    public async Task<IActionResult> GetGameById(string id)
    {
        var game = mapper.Map<GameModel>(await gameService.GetGameByIdAsync(id, IncludeDeleted));

        return Ok(game);
    }

    [HttpGet("platforms/{id}/games")]
    [ResponseCache(Duration = 60)]
    public async Task<IActionResult> GetGamesByPlatform(Guid id)
    {
        var games = mapper.Map<IEnumerable<GameModel>>(
            await gameService.GetGamesByPlatformAsync(id, IncludeDeleted));
        return Ok(games);
    }

    [HttpGet("genres/{id}/games")]
    public async Task<IActionResult> GetGamesByGenre(string id)
    {
        var games = mapper.Map<IEnumerable<GameModel>>(
            await gameService.GetGamesByGenreAsync(id, IncludeDeleted));
        return Ok(games);
    }

    [HttpGet("publishers/{companyName}/games")]
    public async Task<IActionResult> GetGamesByPublisher(string companyName)
    {
        var games = mapper.Map<IEnumerable<GameModel>>(
            await gameService.GetGamesByPublisherAsync(companyName, IncludeDeleted));
        return Ok(games);
    }

    [Authorize(Policy = Policies.CanManageEntities)]
    [HttpPut("games")]
    public async Task<IActionResult> UpdateGame([FromBody] UpdateGameModel gameModel)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var gameDto = mapper.Map<UpdateGameDto>(gameModel);

        await gameService.UpdateGameAsync(gameDto, CanEditDeleted);

        return Ok();
    }

    [Authorize(Policy = Policies.CanManageEntities)]
    [HttpDelete("games/{key}")]
    public async Task<IActionResult> DeleteGame(string key)
    {
        await gameService.DeleteGameAsync(key);

        return NoContent();
    }

    [HttpGet("games/{key}/file")]
    [ResponseCache(Duration = 60)]
    public async Task<IActionResult> DownloadGame(string key)
    {
        var game = await gameService.GetGameByKeyAsync(key, IncludeDeleted);

        string gameContent = JsonConvert.SerializeObject(game, Formatting.Indented);

        string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        string fileName = $"{game.Name}_{timestamp}.txt";

        var fileBytes = System.Text.Encoding.UTF8.GetBytes(gameContent);

        return File(fileBytes, "text/plain", fileName);
    }

    [HttpGet("games/pagination-options")]
    [ResponseCache(Duration = 60)]
    public IActionResult GetPaginationOptions()
    {
        return Ok(PaginationOptions.List);
    }

    [HttpGet("games/sorting-options")]
    [ResponseCache(Duration = 60)]
    public IActionResult GetSortingOptions()
    {
        return Ok(SortingOptions.List);
    }

    [HttpGet("games/publish-date-options")]
    [ResponseCache(Duration = 60)]
    public IActionResult GetPublishDateOptions()
    {
        return Ok(PublishDateOptions.List);
    }

    [HttpGet("games/{key}/image")]
    public async Task<IActionResult> GetGameImage(string key)
    {
        var image = await gameService.GetGameImageAsync(key);

        return image is null ? NoContent() : File(image.File, image.ContentType);
    }
}
