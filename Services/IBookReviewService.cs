using Nha_Gia_Kim.Models.ViewModels;

namespace Nha_Gia_Kim.Services;

public interface IBookReviewService
{
    Task<(IReadOnlyList<BookReviewItemViewModel> Reviews, int Count, decimal AverageRating)>
        GetSummaryAsync(int bookId, CancellationToken cancellationToken);

    Task<bool> AddAsync(int bookId, BookReviewSubmission submission, CancellationToken cancellationToken);
}
