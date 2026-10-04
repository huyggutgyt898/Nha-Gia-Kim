namespace Nha_Gia_Kim.Models.Entities;

public sealed class Book
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public string? Subtitle { get; set; }
    public int AuthorId { get; set; }
    public Author Author { get; set; } = null!;
    public string? Isbn { get; set; }
    public string? Description { get; set; }
    public string? CoverImageUrl { get; set; }
    public string? FilePreviewUrl { get; set; }
    public decimal Price { get; set; }
    public decimal? OriginalPrice { get; set; }
    public int StockQuantity { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTimeOffset CreatedAt { get; set; }
    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;
    public byte[] RowVersion { get; set; } = [];
    public ICollection<BookReview> Reviews { get; set; } = new List<BookReview>();
}
