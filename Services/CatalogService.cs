using Microsoft.EntityFrameworkCore;
using Nha_Gia_Kim.Data;
using Nha_Gia_Kim.Models.Api;
using Nha_Gia_Kim.Models.Entities;

namespace Nha_Gia_Kim.Services;

public sealed class CatalogService(BookstoreDbContext dbContext) : ICatalogService
{
    public async Task<BookListResponse> GetBooksAsync(
        string? search,
        int? categoryId,
        int page,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var query = dbContext.Books
            .AsNoTracking()
            .Where(book => book.IsActive)
            .Include(book => book.Author)
            .Include(book => book.Category)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(book => book.Title.Contains(term) || book.Author.Name.Contains(term));
        }

        if (categoryId.HasValue)
        {
            query = query.Where(book => book.CategoryId == categoryId.Value);
        }

        var totalItems = await query.CountAsync(cancellationToken);
        var books = await query
            .OrderBy(book => book.Title)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new BookListResponse(books.Select(ToResponse).ToArray(), page, pageSize, totalItems);
    }

    public async Task<BookResponse?> GetBookByIdAsync(int id, CancellationToken cancellationToken)
    {
        var book = await dbContext.Books
            .AsNoTracking()
            .Include(item => item.Author)
            .Include(item => item.Category)
            .SingleOrDefaultAsync(item => item.Id == id && item.IsActive, cancellationToken);

        return book is null ? null : ToResponse(book);
    }

    public async Task<IReadOnlyList<CategoryResponse>> GetCategoriesAsync(CancellationToken cancellationToken)
    {
        return await dbContext.Categories
            .AsNoTracking()
            .OrderBy(category => category.Name)
            .Select(category => new CategoryResponse(category.Id, category.Name, category.Slug))
            .ToArrayAsync(cancellationToken);
    }

    private static BookResponse ToResponse(Book book) => new(
        book.Id,
        book.Title,
        book.Author.Name,
        book.Isbn,
        book.Description,
        book.CoverImageUrl,
        book.Price,
        book.StockQuantity,
        book.CategoryId,
        book.Category.Name);
}
