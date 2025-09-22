using AutoMapper;
using GameStore.API.Models.PublisherModels;
using GameStore.Application.DTOs.PublisherDtos;
using GameStore.Application.Services.PublisherService;
using GameStore.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GameStore.API.Controllers;

[Route("api/")]
[ApiController]
public class PublishersController(IPublisherService publisherService, IMapper mapper) : ControllerBase
{
    private bool IncludeDeleted => User.Identity?.IsAuthenticated == true &&
                   User.Claims.Any(c => c.Type == Permissions.PermissionClaim &&
                                        c.Value == Permissions.ViewDeletedGames);

    [Authorize(Policy = Policies.CanManageEntities)]
    [HttpPost("publishers")]
    public async Task<IActionResult> AddPublisher(
        [FromBody] AddPublisherRequestModel publisherRequestModel)
    {
        if (publisherRequestModel == null)
        {
            return BadRequest("Publisher request data can not be null");
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var publisherDto = mapper.Map<AddPublisherDto>(publisherRequestModel.Publisher);

        var publisher = await publisherService.AddPublisherAsync(publisherDto);
        return CreatedAtAction(
            nameof(GetPublisherByName),
            new { companyName = publisher.CompanyName },
            publisher);
    }

    [HttpGet("publishers/{companyName}")]
    public async Task<IActionResult> GetPublisherByName(string companyName)
    {
        var publisher = mapper.Map<PublisherModel>(
            await publisherService.GetPublisherByNameAsync(companyName, IncludeDeleted));

        return Ok(publisher);
    }

    [HttpGet("publishers")]
    public async Task<IActionResult> GetAllPublishers()
    {
        var publishers = mapper.Map<IEnumerable<PublisherModel>>(
            await publisherService.GetAllPublishersAsync(IncludeDeleted));

        return Ok(publishers);
    }

    [HttpGet("games/{key}/publisher")]
    public async Task<IActionResult> GetPublisherByGameKey(string key)
    {
        var publisher = mapper.Map<PublisherModel>(
            await publisherService.GetPublisherByGameKeyAsync(key, IncludeDeleted));

        return Ok(publisher);
    }

    [Authorize(Policy = Policies.CanManageEntities)]
    [HttpPut("publishers")]
    public async Task<IActionResult> UpdatePublisher(
        [FromBody] UpdatePublisherRequestModel publisherRequestModel)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var publisherDto = mapper.Map<PublisherDto>(publisherRequestModel.Publisher);

        await publisherService.UpdatePublisherAsync(publisherDto);

        return Ok();
    }

    [Authorize(Policy = Policies.CanManageEntities)]
    [HttpDelete("publishers/{id}")]
    public async Task<IActionResult> DeletePublisher(string id)
    {
        await publisherService.DeletePublisherAsync(id);

        return NoContent();
    }
}
