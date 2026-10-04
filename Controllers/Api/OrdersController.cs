using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Nha_Gia_Kim.Models.Api;
using Nha_Gia_Kim.Services;

namespace Nha_Gia_Kim.Controllers.Api;

[ApiController]
[Route("api/orders")]
[Produces("application/json")]
public sealed class OrdersController(
    IOrderService orderService,
    IOrderConfirmationEmailSender emailSender,
    ILogger<OrdersController> logger) : ControllerBase
{
    /// <summary>Places an order from a cart and reserves the books from stock.</summary>
    [HttpPost]
    [EnableRateLimiting("order-checkout")]
    [EndpointSummary("Place an order")]
    [EndpointDescription("Validates stock, records a guest order, decrements inventory transactionally, clears the cart, and attempts to email the customer.")]
    [ProducesResponseType<OrderResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<OrderResponse>> Create(
        CreateOrderRequest request,
        CancellationToken cancellationToken)
    {
        var order = await orderService.CreateAsync(request, cancellationToken);
        var emailResult = await emailSender.SendConfirmationAsync(request, order, CancellationToken.None);
        if (!emailResult.Sent)
        {
            logger.LogWarning(
                "Order {OrderNumber} was saved, but its confirmation email was not sent. Reason: {Reason}",
                order.OrderNumber,
                emailResult.FailureReason);
        }

        return CreatedAtAction(
            nameof(GetByNumber),
            new { orderNumber = order.OrderNumber },
            order with { ConfirmationEmailSent = emailResult.Sent });
    }

    /// <summary>Returns a placed order using its unguessable order number.</summary>
    [HttpGet("{orderNumber:guid}", Name = nameof(GetByNumber))]
    [EndpointSummary("Get order confirmation")]
    [EndpointDescription("Returns non-personal order confirmation details using the order number.")]
    [ProducesResponseType<OrderResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<OrderResponse>> GetByNumber(Guid orderNumber, CancellationToken cancellationToken)
    {
        var order = await orderService.GetByNumberAsync(orderNumber, cancellationToken);
        return order is null ? NotFound() : Ok(order);
    }
}
