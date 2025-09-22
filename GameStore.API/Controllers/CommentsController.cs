using AutoMapper;
using GameStore.API.Models.CommentModels;
using GameStore.Application.DTOs.CommentDtos;
using GameStore.Application.DTOs.UserDtos;
using GameStore.Application.Services.CommentServices;
using GameStore.Application.Services.UserServices;
using GameStore.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameStore.API.Controllers;

[Route("api/")]
[ApiController]
public class CommentsController(
    ICommentService commentService,
    IBanService banService,
    IMapper mapper) : ControllerBase
{
    private bool CanManageForDeletedGames => User.Identity?.IsAuthenticated == true &&
                   User.Claims.Any(c => c.Type == Permissions.PermissionClaim &&
                                        c.Value == Permissions.ManageDeletedGameComments);

    [Authorize(Policy = Policies.CanCommentGame)]
    [HttpPost("games/{key}/comments")]
    public async Task<IActionResult> AddCommentByGameKey(
        string key, [FromBody] AddCommentModel commentModel)
    {
        var username = User.Identity?.Name;

        if (string.IsNullOrEmpty(username))
        {
            return Unauthorized("User not found.");
        }

        if (await banService.IsUserBannedAsync(username))
        {
            return StatusCode(StatusCodes.Status403Forbidden, new
            {
                Message = "User is banned from posting comments.",
            });
        }

        var addCommentDto = mapper.Map<AddCommentDto>(commentModel);
        addCommentDto.Name = username;
        addCommentDto.GameKey = key;

        await commentService.AddCommentAsync(addCommentDto);
        return Ok();
    }

    [HttpGet("games/{key}/comments")]
    public async Task<IActionResult> GetAllByGameKey(string key)
    {
        var comments = await commentService.GetAllByGameKey(key);

        return Ok(comments);
    }

    [Authorize(Policy = Policies.CanManageComments)]
    [HttpDelete("games/{key}/comments/{id}")]
    public async Task<IActionResult> DeleteComment(string key, Guid id)
    {
        await commentService.DeleteCommentAsync(key, id, CanManageForDeletedGames);

        return Ok();
    }

    [HttpGet("comments/ban/durations")]
    public IActionResult GetBanDurations()
    {
        var banDurations = BanDurations.List;

        return Ok(banDurations);
    }

    [Authorize(Policy = Policies.CanBanUsers)]
    [HttpPost("comments/ban")]
    public async Task<IActionResult> BanUser([FromBody] BanModel banModel)
    {
        await banService.AddBannedUserAsync(mapper.Map<BanDto>(banModel));
        return Ok();
    }
}
