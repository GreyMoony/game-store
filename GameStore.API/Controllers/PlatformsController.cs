using AutoMapper;
using GameStore.API.Models.PlatformModels;
using GameStore.Application.DTOs.PlatformDtos;
using GameStore.Application.Services.PlatformServices;
using GameStore.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameStore.API.Controllers;
[Route("api/")]
[ApiController]
public class PlatformsController(
    IPlatformService platformService,
    IMapper mapper) : ControllerBase
{
    private bool IncludeDeleted => User.Identity?.IsAuthenticated == true &&
                   User.Claims.Any(c => c.Type == Permissions.PermissionClaim &&
                                        c.Value == Permissions.ViewDeletedGames);

    [Authorize(Policy = Policies.CanManageEntities)]
    [HttpPost("platforms")]
    public async Task<IActionResult> AddPlatform(
        [FromBody] AddPlatformRequestModel platformRequestModel)
    {
        if (platformRequestModel == null)
        {
            return BadRequest("Platform data can not be null");
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var platformDto = mapper.Map<AddPlatformDto>(platformRequestModel.Platform);

        var platform = await platformService.AddPlatformAsync(platformDto);
        return CreatedAtAction(nameof(GetPlatformById), new { id = platform.Id }, platform);
    }

    [HttpGet("platforms/{id}")]
    public async Task<IActionResult> GetPlatformById(Guid id)
    {
        var platform = mapper.Map<PlatformModel>(
            await platformService.GetPlatformByIdAsync(id, IncludeDeleted));

        return Ok(platform);
    }

    [HttpGet("platforms")]
    public async Task<IActionResult> GetAllPlatforms()
    {
        var platforms = mapper.Map<IEnumerable<PlatformModel>>(
            await platformService.GetAllPlatformsAsync(IncludeDeleted));
        return Ok(platforms);
    }

    [HttpGet("games/{key}/platforms")]
    public async Task<IActionResult> GetPlatformsByGameKey(string key)
    {
        var platforms = mapper.Map<IEnumerable<PlatformModel>>(
            await platformService.GetPlatformsByGameKeyAsync(key, IncludeDeleted));

        return Ok(platforms);
    }

    [Authorize(Policy = Policies.CanManageEntities)]
    [HttpPut("platforms")]
    public async Task<IActionResult> UpdatePlatform([FromBody] UpdatePlatformModel platformModel)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var platformDto = mapper.Map<PlatformDto>(platformModel.Platform);

        await platformService.UpdatePlatformAsync(platformDto);

        return Ok();
    }

    [Authorize(Policy = Policies.CanManageEntities)]
    [HttpDelete("platforms/{id}")]
    public async Task<IActionResult> DeletePlatform(Guid id)
    {
        await platformService.DeletePlatformAsync(id);

        return NoContent();
    }
}
