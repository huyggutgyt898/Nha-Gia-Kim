namespace Nha_Gia_Kim.Models.Entities;

public sealed class Category
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Slug { get; set; }
    public ICollection<Book> Books { get; } = new List<Book>();
}
