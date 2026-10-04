namespace Nha_Gia_Kim.Models.Entities;

public sealed class BookReview
{
    public int Id { get; set; }
    public int BookId { get; set; }
    public required string DisplayName { get; set; }
    public int Rating { get; set; }
    public string? Content { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public Book Book { get; set; } = null!;
}
