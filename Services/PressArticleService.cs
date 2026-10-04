using Microsoft.EntityFrameworkCore;
using Nha_Gia_Kim.Data;
using Nha_Gia_Kim.Models.ViewModels;

namespace Nha_Gia_Kim.Services;

public sealed class PressArticleService(BookstoreDbContext dbContext) : IPressArticleService
{
    public async Task<IReadOnlyList<PressArticleItemViewModel>> GetForBookAsync(
        int bookId,
        CancellationToken cancellationToken)
    {
        var articles = await dbContext.PressArticles
            .AsNoTracking()
            .Where(article => article.BookId == bookId && article.Book.IsActive)
            .OrderByDescending(article => article.Id)
            .Select(article => new PressArticleItemViewModel(
                article.PressName,
                article.ArticleTitle,
                article.ArticleUrl,
                article.Snippet))
            .ToArrayAsync(cancellationToken);

        return articles
            .Select(article => article with { ArticleUrl = GetSafeArticleUrl(article.ArticleUrl) })
            .ToArray();
    }

    private static string? GetSafeArticleUrl(string? value)
    {
        return Uri.TryCreate(value, UriKind.Absolute, out var uri)
            && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps)
                ? uri.AbsoluteUri
                : null;
    }
}
