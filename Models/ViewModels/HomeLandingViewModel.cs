using Nha_Gia_Kim.Models.Api;

namespace Nha_Gia_Kim.Models.ViewModels;

public sealed record HomeLandingViewModel(
    BookListResponse Books,
    IReadOnlyList<BookReviewItemViewModel> Reviews,
    int ReviewCount,
    decimal AverageRating,
    IReadOnlyList<BookResponse> RelatedBooks,
    IReadOnlyList<PressArticleItemViewModel> PressArticles,
    BookReviewSubmission ReviewForm);
