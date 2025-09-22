using System.Security.Claims;
using AutoMapper;
using GameStore.API.Helpers;
using GameStore.API.Models.OrderModels;
using GameStore.Application.DTOs.OrderDtos;
using GameStore.Application.Services.OrderServices;
using GameStore.Application.Services.PaymentStrategies;
using GameStore.Domain.Constants;
using GameStore.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace GameStore.API.Controllers;

[Route("api/")]
[ApiController]
public class OrdersController(IOrderService orderService, IMapper mapper) : ControllerBase
{
    [Authorize(Policy = Policies.CanViewOrdersHistory)]
    [HttpGet("orders/history")]
    public async Task<IActionResult> GetOrdersHistory([FromQuery] string? start, [FromQuery] string? end)
    {
        DateTime? startDate = DateTimeParser.ParseDate(start);
        DateTime? endDate = DateTimeParser.ParseDate(end);

        if (startDate.HasValue && endDate.HasValue && startDate > endDate)
        {
            return BadRequest("Start date cannot be after end date.");
        }

        var orders = mapper.Map<IEnumerable<OrderModel>>(await orderService
            .GetOrderHistory(startDate, endDate));
        return Ok(orders);
    }

    [HttpGet("orders")]
    public async Task<IActionResult> GetPaidAndCancelledOrders()
    {
        var orders = mapper.Map<IEnumerable<OrderModel>>(await orderService
            .GetPaidAndCancelledOrders());
        return Ok(orders);
    }

    [HttpGet("orders/{id}")]
    public async Task<IActionResult> GetOrderById(string id)
    {
        var order = mapper.Map<OrderModel>(await orderService.GetOrderById(id));
        return Ok(order);
    }

    [HttpGet("orders/{id}/details")]
    public async Task<IActionResult> GetOrderDetails(string id)
    {
        var orderDetails = mapper.Map<IEnumerable<OrderDetailsModel>>(
            await orderService.GetOrderDetails(id));
        return Ok(orderDetails);
    }

    [HttpGet("orders/cart")]
    public async Task<IActionResult> GetCart()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized("User ID not found in token.");
        }

        var cart = await orderService.GetCartAsync(Guid.Parse(userId));
        var cartModel = cart.Details.IsNullOrEmpty() ?
            [] :
            mapper.Map<IEnumerable<OrderDetailsModel>>(cart.Details);

        return Ok(cartModel);
    }

    [HttpGet("orders/payment-methods")]
    public IActionResult GetPaymentMethods()
    {
        var methods = mapper.Map<IEnumerable<PaymentMethodModel>>(
            orderService.GetPaymentMethods());
        return Ok(new PaymentMethodsResponseModel { PaymentMethods = methods });
    }

    [HttpPost("games/{key}/buy")]
    public async Task<IActionResult> AddGameToCart(string key)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized("User ID not found in token.");
        }

        await orderService.AddGameToCartAsync(Guid.Parse(userId), key);

        return Ok();
    }

    [HttpDelete("orders/cart/{key}")]
    public async Task<IActionResult> DeleteGameFromCart(string key)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized("User ID not found in token.");
        }

        await orderService.RemoveGameFromCartAsync(Guid.Parse(userId), key);

        return Ok();
    }

    [HttpPost("orders/payment")]
    public async Task<IActionResult> ProcessPayment([FromBody] PaymentRequestModel requestModel)
    {
        if (!PaymentMethods.IsSupported(requestModel.Method))
        {
            return BadRequest("Invalid payment method.");
        }

        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized("User ID not found in token.");
        }

        var requestDto = mapper.Map<PaymentRequestDto>(requestModel);
        var paymentResult = await orderService.ProcessOrderPaymentAsync(Guid.Parse(userId), requestDto);

        if (paymentResult.ResultType is PaymentResultType.PaymentSuccess)
        {
            return Ok(paymentResult.Response);
        }
        else if (paymentResult.ResultType is PaymentResultType.InvoiceGenerated)
        {
            return File(paymentResult.Invoice!, "application/pdf", "invoice.pdf");
        }

        return BadRequest(paymentResult.ErrorMessage);
    }

    [Authorize(Policy = Policies.CanEditOrders)]
    [HttpPatch("orders/details/{id}/quantity")]
    public async Task<IActionResult> UpdateOrderDetailQuantity(
    Guid id, [FromBody] OrderDetailQuantityUpdateRequest request)
    {
        await orderService.UpdateOrderDetailQuantityAsync(id, request.Count);
        return NoContent();
    }

    [Authorize(Policy = Policies.CanEditOrders)]
    [HttpDelete("orders/details/{id}")]
    public async Task<IActionResult> DeleteOrderDetail(Guid id)
    {
        await orderService.DeleteOrderDetailAsync(id);
        return NoContent();
    }

    [Authorize(Policy = Policies.CanEditOrders)]
    [HttpPost("orders/{id}/ship")]
    public async Task<IActionResult> ShipOrder(Guid id)
    {
        await orderService.ChangeStatusToShipped(id);

        return NoContent();
    }

    [Authorize(Policy = Policies.CanEditOrders)]
    [HttpPost("orders/{id}/details/{key}")]
    public async Task<IActionResult> AddGameAsOrderDetail(Guid id, string key)
    {
        await orderService.AddGameToOrder(id, key);

        return NoContent();
    }
}
