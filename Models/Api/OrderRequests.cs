using System.ComponentModel.DataAnnotations;

namespace Nha_Gia_Kim.Models.Api;

/// <summary>Payload for placing an order from an existing anonymous cart.</summary>
public sealed record CreateOrderRequest
{
    /// <summary>Identifier returned when the cart was created.</summary>
    public Guid CartId { get; init; }

    /// <summary>Customer name for delivery.</summary>
    [Required, StringLength(150, MinimumLength = 2)]
    public required string CustomerName { get; init; }

    /// <summary>Customer email address.</summary>
    [Required, EmailAddress, StringLength(254)]
    public required string CustomerEmail { get; init; }

    /// <summary>Customer contact phone number.</summary>
    [Required, StringLength(30, MinimumLength = 8)]
    public required string CustomerPhone { get; init; }

    /// <summary>Full delivery address.</summary>
    [Required, StringLength(500, MinimumLength = 8)]
    public required string ShippingAddress { get; init; }

    /// <summary>Province or city for delivery.</summary>
    [Required, StringLength(100, MinimumLength = 2)]
    public required string ProvinceCity { get; init; }

    /// <summary>Selected payment method.</summary>
    [Required, RegularExpression("^(CashOnDelivery|BankTransfer|DigitalWallet)$")]
    public required string PaymentMethod { get; init; }
}

/// <summary>A line captured on a placed order.</summary>
public sealed record OrderItemResponse(int BookId, string BookTitle, int Quantity, decimal UnitPrice, decimal LineTotal);

/// <summary>Confirmation details returned after an order is placed.</summary>
public sealed record OrderResponse(
    Guid OrderNumber,
    string Status,
    decimal TotalAmount,
    DateTimeOffset CreatedAt,
    IReadOnlyList<OrderItemResponse> Items)
{
    /// <summary>Whether the confirmation email was sent after the order was saved; null on order lookup.</summary>
    public bool? ConfirmationEmailSent { get; init; }
}
