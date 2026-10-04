namespace Nha_Gia_Kim.Models.Entities;

public sealed class CustomerOrder
{
    public int Id { get; set; }
    public Guid OrderNumber { get; set; }
    public required string CustomerName { get; set; }
    public required string CustomerEmail { get; set; }
    public required string CustomerPhone { get; set; }
    public required string ShippingAddress { get; set; }
    public required string ProvinceCity { get; set; }
    public required string PaymentMethod { get; set; }
    public required string Status { get; set; }
    public decimal TotalAmount { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public ICollection<OrderItem> Items { get; } = new List<OrderItem>();
}
