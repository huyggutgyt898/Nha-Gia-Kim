namespace Nha_Gia_Kim.Models.Entities;

public sealed class PressArticle
{
    public int Id { get; set; }
    public int BookId { get; set; }
    public Book Book { get; set; } = null!;
    public required string PressName { get; set; }
    public required string ArticleTitle { get; set; }
    public string? ArticleUrl { get; set; }
    public string? Snippet { get; set; }
}
