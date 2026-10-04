namespace Nha_Gia_Kim.Models.Api;

/// <summary>Book details available in the public catalog.</summary>
public sealed record BookResponse(
    int Id,
    string Title,
    string Author,
    string? Isbn,
    string? Description,
    string? CoverImageUrl,
    decimal Price,
    int StockQuantity,
    int CategoryId,
    string CategoryName);

/// <summary>A page of books returned by the catalog.</summary>
public sealed record BookListResponse(IReadOnlyList<BookResponse> Items, int Page, int PageSize, int TotalItems);

/// <summary>A book category available in the catalog.</summary>
public sealed record CategoryResponse(int Id, string Name, string Slug);
