using System.Globalization;
using System.Text;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Nha_Gia_Kim.Data;
using Nha_Gia_Kim.Models.Entities;
using Nha_Gia_Kim.Models.ViewModels;

namespace Nha_Gia_Kim.Services;

public sealed class AdminManagementService(BookstoreDbContext dbContext) : IAdminManagementService
{
    public async Task<AdminBooksViewModel> GetBooksAsync(
        string? search,
        CancellationToken cancellationToken)
    {
        var query = dbContext.Books
            .AsNoTracking()
            .Include(book => book.Author)
            .Include(book => book.Category)
            .AsQueryable();

        var normalizedSearch = search?.Trim();
        if (!string.IsNullOrWhiteSpace(normalizedSearch))
        {
            query = query.Where(book =>
                book.Title.Contains(normalizedSearch)
                || book.Author.Name.Contains(normalizedSearch)
                || (book.Isbn != null && book.Isbn.Contains(normalizedSearch)));
        }

        var totalBooks = await query.CountAsync(cancellationToken);
        var books = await query
            .OrderBy(book => book.Title)
            .Select(book => new AdminBookRowViewModel(
                book.Id,
                book.Title,
                book.Author.Name,
                book.Category.Name,
                book.Price,
                book.StockQuantity,
                book.IsActive,
                book.CoverImageUrl))
            .ToArrayAsync(cancellationToken);

        return new AdminBooksViewModel(books, normalizedSearch, totalBooks);
    }

    public async Task<AdminBookFormViewModel?> GetBookFormAsync(
        int? id,
        CancellationToken cancellationToken)
    {
        AdminBookFormViewModel model;
        if (id is null)
        {
            model = new AdminBookFormViewModel();
        }
        else
        {
            var book = await dbContext.Books
                .AsNoTracking()
                .SingleOrDefaultAsync(item => item.Id == id, cancellationToken);
            if (book is null)
            {
                return null;
            }

            model = new AdminBookFormViewModel
            {
                Id = book.Id,
                Title = book.Title,
                Subtitle = book.Subtitle,
                AuthorId = book.AuthorId,
                CategoryId = book.CategoryId,
                Price = book.Price,
                OriginalPrice = book.OriginalPrice,
                Isbn = book.Isbn,
                CoverImageUrl = book.CoverImageUrl,
                FilePreviewUrl = book.FilePreviewUrl,
                Description = book.Description,
                StockQuantity = book.StockQuantity,
                IsActive = book.IsActive,
                RowVersion = Convert.ToBase64String(book.RowVersion)
            };
        }

        await PopulateBookOptionsAsync(model, cancellationToken);
        return model;
    }

    public async Task PopulateBookOptionsAsync(
        AdminBookFormViewModel model,
        CancellationToken cancellationToken)
    {
        model.Authors = await dbContext.Authors
            .AsNoTracking()
            .OrderBy(author => author.Name)
            .Select(author => new AdminOptionViewModel(author.Id, author.Name))
            .ToArrayAsync(cancellationToken);
        model.Categories = await dbContext.Categories
            .AsNoTracking()
            .OrderBy(category => category.Name)
            .Select(category => new AdminOptionViewModel(category.Id, category.Name))
            .ToArrayAsync(cancellationToken);
    }

    public async Task<AdminSaveResult> SaveBookAsync(
        int? id,
        AdminBookFormViewModel model,
        CancellationToken cancellationToken)
    {
        if (!IsSafeLink(model.CoverImageUrl) || !IsSafeLink(model.FilePreviewUrl))
        {
            return AdminSaveResult.InvalidInput;
        }

        if (!await dbContext.Authors.AnyAsync(author => author.Id == model.AuthorId, cancellationToken)
            || !await dbContext.Categories.AnyAsync(category => category.Id == model.CategoryId, cancellationToken))
        {
            return AdminSaveResult.InvalidReference;
        }

        Book book;
        if (id is null)
        {
            book = new Book
            {
                Title = string.Empty,
                AuthorId = model.AuthorId,
                CategoryId = model.CategoryId,
                CreatedAt = DateTimeOffset.UtcNow
            };
            dbContext.Books.Add(book);
        }
        else
        {
            var existingBook = await dbContext.Books
                .SingleOrDefaultAsync(item => item.Id == id, cancellationToken);
            if (existingBook is null)
            {
                return AdminSaveResult.NotFound;
            }

            book = existingBook;

            byte[] originalRowVersion;
            try
            {
                originalRowVersion = Convert.FromBase64String(model.RowVersion ?? string.Empty);
            }
            catch (FormatException)
            {
                return AdminSaveResult.Conflict;
            }

            if (originalRowVersion.Length != 8)
            {
                return AdminSaveResult.Conflict;
            }

            dbContext.Entry(book).Property(item => item.RowVersion).OriginalValue = originalRowVersion;
        }

        book.Title = model.Title.Trim();
        book.Subtitle = ToNullIfWhiteSpace(model.Subtitle);
        book.AuthorId = model.AuthorId;
        book.CategoryId = model.CategoryId;
        book.Price = model.Price;
        book.OriginalPrice = model.OriginalPrice;
        book.Isbn = ToNullIfWhiteSpace(model.Isbn);
        book.CoverImageUrl = ToNullIfWhiteSpace(model.CoverImageUrl);
        book.FilePreviewUrl = ToNullIfWhiteSpace(model.FilePreviewUrl);
        book.Description = ToNullIfWhiteSpace(model.Description);
        book.StockQuantity = model.StockQuantity;
        book.IsActive = model.IsActive;

        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);
            return AdminSaveResult.Success;
        }
        catch (DbUpdateConcurrencyException)
        {
            return AdminSaveResult.Conflict;
        }
    }

    public async Task<AdminSaveResult> ToggleBookAvailabilityAsync(
        int id,
        CancellationToken cancellationToken)
    {
        var book = await dbContext.Books.SingleOrDefaultAsync(item => item.Id == id, cancellationToken);
        if (book is null)
        {
            return AdminSaveResult.NotFound;
        }

        book.IsActive = !book.IsActive;
        await dbContext.SaveChangesAsync(cancellationToken);
        return AdminSaveResult.Success;
    }

    public async Task<AdminSaveResult> DeleteBookAsync(int id, CancellationToken cancellationToken)
    {
        await using var transaction = await dbContext.Database.BeginTransactionAsync(
            System.Data.IsolationLevel.Serializable,
            cancellationToken);
        var book = await dbContext.Books.SingleOrDefaultAsync(item => item.Id == id, cancellationToken);
        if (book is null)
        {
            return AdminSaveResult.NotFound;
        }

        var hasOrderItems = await dbContext.OrderItems.AnyAsync(item => item.BookId == id, cancellationToken);
        var hasFeedback = await dbContext.Feedbacks.AnyAsync(item => item.BookId == id, cancellationToken);
        var hasPressArticles = await dbContext.PressArticles.AnyAsync(item => item.BookId == id, cancellationToken);
        if (hasOrderItems || hasFeedback || hasPressArticles)
        {
            return AdminSaveResult.InUse;
        }

        dbContext.Books.Remove(book);
        await dbContext.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        return AdminSaveResult.Success;
    }

    public async Task<AdminContentViewModel> GetContentAsync(
        string? tab,
        CancellationToken cancellationToken)
    {
        var articles = await dbContext.PressArticles
            .AsNoTracking()
            .OrderBy(article => article.ArticleTitle)
            .Select(article => new AdminArticleRowViewModel(
                article.Id,
                article.PressName,
                article.ArticleTitle,
                article.ArticleUrl,
                article.Book.Title))
            .ToArrayAsync(cancellationToken);
        var authors = await dbContext.Authors
            .AsNoTracking()
            .OrderBy(author => author.Name)
            .Select(author => new AdminAuthorRowViewModel(
                author.Id,
                author.Name,
                author.Bio,
                author.AvatarUrl,
                author.Books.Count))
            .ToArrayAsync(cancellationToken);
        var categories = await dbContext.Categories
            .AsNoTracking()
            .OrderBy(category => category.Name)
            .Select(category => new AdminCategoryRowViewModel(
                category.Id,
                category.Name,
                category.Slug,
                category.Books.Count))
            .ToArrayAsync(cancellationToken);

        var activeTab = tab is "authors" or "categories" ? tab : "articles";
        return new AdminContentViewModel(activeTab, articles, authors, categories);
    }

    public async Task<AdminAuthorFormViewModel?> GetAuthorFormAsync(
        int? id,
        CancellationToken cancellationToken)
    {
        if (id is null)
        {
            return new AdminAuthorFormViewModel();
        }

        var author = await dbContext.Authors
            .AsNoTracking()
            .SingleOrDefaultAsync(item => item.Id == id, cancellationToken);
        return author is null
            ? null
            : new AdminAuthorFormViewModel
            {
                Id = author.Id,
                Name = author.Name,
                Bio = author.Bio,
                AvatarUrl = author.AvatarUrl
            };
    }

    public async Task<AdminSaveResult> SaveAuthorAsync(
        int? id,
        AdminAuthorFormViewModel model,
        CancellationToken cancellationToken)
    {
        if (!IsSafeLink(model.AvatarUrl))
        {
            return AdminSaveResult.InvalidInput;
        }

        Author author;
        if (id is null)
        {
            author = new Author { Name = string.Empty };
            dbContext.Authors.Add(author);
        }
        else
        {
            var existingAuthor = await dbContext.Authors
                .SingleOrDefaultAsync(item => item.Id == id, cancellationToken);
            if (existingAuthor is null)
            {
                return AdminSaveResult.NotFound;
            }

            author = existingAuthor;
        }

        author.Name = model.Name.Trim();
        author.Bio = ToNullIfWhiteSpace(model.Bio);
        author.AvatarUrl = ToNullIfWhiteSpace(model.AvatarUrl);
        await dbContext.SaveChangesAsync(cancellationToken);
        return AdminSaveResult.Success;
    }

    public async Task<AdminSaveResult> DeleteAuthorAsync(int id, CancellationToken cancellationToken)
    {
        var author = await dbContext.Authors.SingleOrDefaultAsync(item => item.Id == id, cancellationToken);
        if (author is null)
        {
            return AdminSaveResult.NotFound;
        }

        if (await dbContext.Books.AnyAsync(book => book.AuthorId == id, cancellationToken))
        {
            return AdminSaveResult.InUse;
        }

        dbContext.Authors.Remove(author);
        await dbContext.SaveChangesAsync(cancellationToken);
        return AdminSaveResult.Success;
    }

    public async Task<AdminCategoryFormViewModel?> GetCategoryFormAsync(
        int? id,
        CancellationToken cancellationToken)
    {
        if (id is null)
        {
            return new AdminCategoryFormViewModel();
        }

        var category = await dbContext.Categories
            .AsNoTracking()
            .SingleOrDefaultAsync(item => item.Id == id, cancellationToken);
        return category is null
            ? null
            : new AdminCategoryFormViewModel { Id = category.Id, Name = category.Name };
    }

    public async Task<AdminSaveResult> SaveCategoryAsync(
        int? id,
        AdminCategoryFormViewModel model,
        CancellationToken cancellationToken)
    {
        var category = id is null
            ? new Category { Name = string.Empty, Slug = string.Empty }
            : await dbContext.Categories.SingleOrDefaultAsync(item => item.Id == id, cancellationToken);
        if (category is null)
        {
            return AdminSaveResult.NotFound;
        }

        var name = model.Name.Trim();
        var baseSlug = CreateSlug(name);
        var slug = baseSlug;
        for (var suffix = 2; await dbContext.Categories.AnyAsync(
                   item => item.Id != id && item.Slug == slug,
                   cancellationToken); suffix++)
        {
            var suffixText = $"-{suffix}";
            slug = $"{baseSlug[..Math.Min(baseSlug.Length, 120 - suffixText.Length)]}{suffixText}";
        }

        category.Name = name;
        category.Slug = slug;
        if (id is null)
        {
            dbContext.Categories.Add(category);
        }

        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);
            return AdminSaveResult.Success;
        }
        catch (DbUpdateException exception) when (IsUniqueViolation(exception))
        {
            return AdminSaveResult.Conflict;
        }
    }

    public async Task<AdminSaveResult> DeleteCategoryAsync(int id, CancellationToken cancellationToken)
    {
        var category = await dbContext.Categories.SingleOrDefaultAsync(item => item.Id == id, cancellationToken);
        if (category is null)
        {
            return AdminSaveResult.NotFound;
        }

        if (await dbContext.Books.AnyAsync(book => book.CategoryId == id, cancellationToken))
        {
            return AdminSaveResult.InUse;
        }

        dbContext.Categories.Remove(category);
        await dbContext.SaveChangesAsync(cancellationToken);
        return AdminSaveResult.Success;
    }

    public async Task<AdminArticleFormViewModel?> GetArticleFormAsync(
        int? id,
        CancellationToken cancellationToken)
    {
        AdminArticleFormViewModel model;
        if (id is null)
        {
            model = new AdminArticleFormViewModel();
        }
        else
        {
            var article = await dbContext.PressArticles
                .AsNoTracking()
                .SingleOrDefaultAsync(item => item.Id == id, cancellationToken);
            if (article is null)
            {
                return null;
            }

            model = new AdminArticleFormViewModel
            {
                Id = article.Id,
                BookId = article.BookId,
                PressName = article.PressName,
                ArticleTitle = article.ArticleTitle,
                ArticleUrl = article.ArticleUrl,
                Snippet = article.Snippet
            };
        }

        await PopulateArticleBooksAsync(model, cancellationToken);
        return model;
    }

    public async Task PopulateArticleBooksAsync(
        AdminArticleFormViewModel model,
        CancellationToken cancellationToken)
    {
        model.Books = await dbContext.Books
            .AsNoTracking()
            .OrderBy(book => book.Title)
            .Select(book => new AdminOptionViewModel(book.Id, book.Title))
            .ToArrayAsync(cancellationToken);
    }

    public async Task<AdminSaveResult> SaveArticleAsync(
        int? id,
        AdminArticleFormViewModel model,
        CancellationToken cancellationToken)
    {
        if (!IsSafeLink(model.ArticleUrl))
        {
            return AdminSaveResult.InvalidInput;
        }

        if (!await dbContext.Books.AnyAsync(book => book.Id == model.BookId, cancellationToken))
        {
            return AdminSaveResult.InvalidReference;
        }

        PressArticle article;
        if (id is null)
        {
            article = new PressArticle
            {
                BookId = model.BookId,
                PressName = string.Empty,
                ArticleTitle = string.Empty
            };
            dbContext.PressArticles.Add(article);
        }
        else
        {
            var existingArticle = await dbContext.PressArticles
                .SingleOrDefaultAsync(item => item.Id == id, cancellationToken);
            if (existingArticle is null)
            {
                return AdminSaveResult.NotFound;
            }

            article = existingArticle;
        }

        article.BookId = model.BookId;
        article.PressName = model.PressName.Trim();
        article.ArticleTitle = model.ArticleTitle.Trim();
        article.ArticleUrl = ToNullIfWhiteSpace(model.ArticleUrl);
        article.Snippet = ToNullIfWhiteSpace(model.Snippet);
        await dbContext.SaveChangesAsync(cancellationToken);
        return AdminSaveResult.Success;
    }

    public async Task<AdminSaveResult> DeleteArticleAsync(int id, CancellationToken cancellationToken)
    {
        var article = await dbContext.PressArticles
            .SingleOrDefaultAsync(item => item.Id == id, cancellationToken);
        if (article is null)
        {
            return AdminSaveResult.NotFound;
        }

        dbContext.PressArticles.Remove(article);
        await dbContext.SaveChangesAsync(cancellationToken);
        return AdminSaveResult.Success;
    }

    public async Task<AdminFeedbackViewModel> GetFeedbackAsync(
        string? search,
        CancellationToken cancellationToken)
    {
        var query = dbContext.Feedbacks
            .AsNoTracking()
            .Include(feedback => feedback.Book)
            .AsQueryable();
        var normalizedSearch = search?.Trim();
        if (!string.IsNullOrWhiteSpace(normalizedSearch))
        {
            query = query.Where(feedback =>
                feedback.DisplayName.Contains(normalizedSearch)
                || (feedback.Content != null && feedback.Content.Contains(normalizedSearch))
                || feedback.Book.Title.Contains(normalizedSearch));
        }

        var totalFeedbacks = await query.CountAsync(cancellationToken);
        var feedbacks = await query
            .OrderByDescending(feedback => feedback.CreatedAt)
            .Take(100)
            .Select(feedback => new AdminFeedbackRowViewModel(
                feedback.Id,
                feedback.DisplayName,
                feedback.Rating,
                feedback.Content,
                feedback.CreatedAt,
                feedback.Book.Title))
            .ToArrayAsync(cancellationToken);

        return new AdminFeedbackViewModel(feedbacks, normalizedSearch, totalFeedbacks);
    }

    public async Task<AdminSaveResult> DeleteFeedbackAsync(int id, CancellationToken cancellationToken)
    {
        var feedback = await dbContext.Feedbacks.SingleOrDefaultAsync(item => item.Id == id, cancellationToken);
        if (feedback is null)
        {
            return AdminSaveResult.NotFound;
        }

        dbContext.Feedbacks.Remove(feedback);
        await dbContext.SaveChangesAsync(cancellationToken);
        return AdminSaveResult.Success;
    }

    private static bool IsSafeLink(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return true;
        }

        var link = value.Trim();
        if (link.StartsWith('/') && !link.StartsWith("//", StringComparison.Ordinal) && !link.Contains('\\'))
        {
            return true;
        }

        return Uri.TryCreate(link, UriKind.Absolute, out var uri)
            && uri.Scheme is "http" or "https";
    }

    private static string? ToNullIfWhiteSpace(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();

    private static string CreateSlug(string value)
    {
        var normalized = value.Replace('đ', 'd').Replace('Đ', 'D').Normalize(NormalizationForm.FormD);
        var slug = new StringBuilder(normalized.Length);
        var lastWasSeparator = false;

        foreach (var character in normalized)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(character) == UnicodeCategory.NonSpacingMark)
            {
                continue;
            }

            if (char.IsLetterOrDigit(character))
            {
                slug.Append(char.ToLowerInvariant(character));
                lastWasSeparator = false;
            }
            else if (!lastWasSeparator && slug.Length > 0)
            {
                slug.Append('-');
                lastWasSeparator = true;
            }
        }

        var result = slug.ToString().Trim('-');
        return result.Length == 0 ? "the-loai" : result[..Math.Min(result.Length, 120)].TrimEnd('-');
    }

    private static bool IsUniqueViolation(DbUpdateException exception) =>
        exception.InnerException is SqlException { Number: 2601 or 2627 };
}
