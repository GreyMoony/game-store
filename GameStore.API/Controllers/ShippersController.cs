using GameStore.DAL.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GameStore.API.Controllers;

[Route("api/")]
[ApiController]
public class ShippersController(IShipperRepository repository) : ControllerBase
{
    [HttpGet("shippers")]
    public async Task<IActionResult> GetAllShippers()
    {
        var shippers = await repository.GetAll();

        return Ok(shippers);
    }
}
