using Microsoft.EntityFrameworkCore;
using Nha_Gia_Kim.Data;
using Nha_Gia_Kim.Models.Entities;
using Nha_Gia_Kim.Models.ViewModels;

namespace Nha_Gia_Kim.Services;

public sealed class BookReviewService(BookstoreDbContext dbContext) : IBookReviewService
{
    public async Task<(IReadOnlyList<BookReviewItemViewModel> Reviews, int Count, decimal AverageRating)>
        GetSummaryAsync(int bookId, CancellationToken cancellationToken)
    {
        var query = dbContext.Feedbacks
            .AsNoTracking()
            .Where(review => review.BookId == bookId);
        var count = await query.CountAsync(cancellationToken);
        var averageRating = count == 0
            ? 0m
            : await query.AverageAsync(review => (decimal)review.Rating, cancellationToken);
        var reviews = await query
            .OrderByDescending(review => review.CreatedAt)
            .Take(6)
            .Select(review => new BookReviewItemViewModel(
                review.DisplayName,
                review.Rating,
                review.Content ?? string.Empty,
                review.CreatedAt))
            .ToArrayAsync(cancellationToken);

        return (reviews, count, averageRating);
    }

    public async Task<bool> AddAsync(
        int bookId,
        BookReviewSubmission submission,
        CancellationToken cancellationToken)
    {
        var bookExists = await dbContext.Books
            .AnyAsync(book => book.Id == bookId && book.IsActive, cancellationToken);
        if (!bookExists)
        {
            return false;
        }

        dbContext.Feedbacks.Add(new BookReview
        {
            BookId = bookId,
            DisplayName = string.IsNullOrWhiteSpace(submission.DisplayName)
                ? "Bạn đọc"
                : submission.DisplayName.Trim(),
            Rating = submission.Rating,
            Content = submission.Content.Trim(),
            CreatedAt = DateTimeOffset.UtcNow
        });

        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }
}
