namespace Nha_Gia_Kim.Models.Entities;

public sealed class Author
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public string? Bio { get; set; }
    public string? AvatarUrl { get; set; }
    public ICollection<Book> Books { get; } = new List<Book>();
}
