using Microsoft.AspNetCore.Mvc;
using Nha_Gia_Kim.Models.Api;
using Nha_Gia_Kim.Services;

namespace Nha_Gia_Kim.Controllers.Api;

[ApiController]
[Route("api/carts/{cartId:guid}")]
[Produces("application/json")]
public sealed class CartsController(ICartService cartService) : ControllerBase
{
    /// <summary>Returns the contents of an anonymous cart.</summary>
    [HttpGet]
    [EndpointSummary("Get a cart")]
    [EndpointDescription("Returns cart lines and their current prices for the specified anonymous cart identifier.")]
    [ProducesResponseType<CartResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CartResponse>> Get(Guid cartId, CancellationToken cancellationToken)
    {
        return Ok(await cartService.GetAsync(cartId, cancellationToken));
    }

    /// <summary>Adds a book to an anonymous cart, creating the cart if needed.</summary>
    [HttpPost("items")]
    [EndpointSummary("Add a book to cart")]
    [EndpointDescription("Adds a quantity of an active book to the cart, creating the cart if it does not exist.")]
    [ProducesResponseType<CartResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<CartResponse>> AddItem(
        Guid cartId,
        AddCartItemRequest request,
        CancellationToken cancellationToken)
    {
        return Ok(await cartService.AddItemAsync(cartId, request, cancellationToken));
    }

    /// <summary>Replaces the quantity of a book in a cart.</summary>
    [HttpPut("items/{bookId:int}")]
    [EndpointSummary("Update cart quantity")]
    [EndpointDescription("Sets a cart line quantity if the requested amount is currently in stock.")]
    [ProducesResponseType<CartResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<CartResponse>> UpdateItem(
        Guid cartId,
        int bookId,
        UpdateCartItemRequest request,
        CancellationToken cancellationToken)
    {
        return Ok(await cartService.UpdateItemAsync(cartId, bookId, request, cancellationToken));
    }

    /// <summary>Removes a book from a cart.</summary>
    [HttpDelete("items/{bookId:int}")]
    [EndpointSummary("Remove a book from cart")]
    [EndpointDescription("Removes the specified book line from the anonymous cart.")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveItem(Guid cartId, int bookId, CancellationToken cancellationToken)
    {
        await cartService.RemoveItemAsync(cartId, bookId, cancellationToken);
        return NoContent();
    }
}
