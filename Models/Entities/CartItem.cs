namespace Nha_Gia_Kim.Models.Entities;

public sealed class CartItem
{
    public Guid CartId { get; set; }
    public Cart Cart { get; set; } = null!;
    public int BookId { get; set; }
    public Book Book { get; set; } = null!;
    public int Quantity { get; set; }
}
