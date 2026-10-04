using Nha_Gia_Kim.Models.Api;

namespace Nha_Gia_Kim.Services;

public interface ICatalogService
{
    Task<BookListResponse> GetBooksAsync(string? search, int? categoryId, int page, int pageSize, CancellationToken cancellationToken);
    Task<BookResponse?> GetBookByIdAsync(int id, CancellationToken cancellationToken);
    Task<IReadOnlyList<CategoryResponse>> GetCategoriesAsync(CancellationToken cancellationToken);
}
