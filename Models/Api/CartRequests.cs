using System.ComponentModel.DataAnnotations;

namespace Nha_Gia_Kim.Models.Api;

/// <summary>Payload for adding a book to an anonymous cart.</summary>
public sealed record AddCartItemRequest
{
    /// <summary>Catalog book identifier.</summary>
    [Range(1, int.MaxValue)]
    public int BookId { get; init; }

    /// <summary>Number of copies to add.</summary>
    [Range(1, 100)]
    public int Quantity { get; init; }
}

/// <summary>Payload for replacing the quantity of a cart line.</summary>
public sealed record UpdateCartItemRequest
{
    /// <summary>New number of copies.</summary>
    [Range(1, 100)]
    public int Quantity { get; init; }
}

/// <summary>One book and its current price in a cart.</summary>
public sealed record CartItemResponse(
    int BookId,
    string Title,
    string Author,
    string? CoverImageUrl,
    decimal UnitPrice,
    int Quantity,
    decimal LineTotal,
    int AvailableStock);

/// <summary>Anonymous cart contents and current subtotal.</summary>
public sealed record CartResponse(Guid CartId, IReadOnlyList<CartItemResponse> Items, decimal Subtotal);
