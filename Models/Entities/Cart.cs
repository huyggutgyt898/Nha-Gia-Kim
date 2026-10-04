namespace Nha_Gia_Kim.Models.Entities;

public sealed class Cart
{
    public Guid Id { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public ICollection<CartItem> Items { get; } = new List<CartItem>();
}
