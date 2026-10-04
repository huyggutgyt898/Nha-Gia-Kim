namespace Nha_Gia_Kim.Models.Entities;

public sealed class OrderItem
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public CustomerOrder Order { get; set; } = null!;
    public int BookId { get; set; }
    public required string BookTitle { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}
