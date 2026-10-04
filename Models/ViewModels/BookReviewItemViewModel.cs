namespace Nha_Gia_Kim.Models.ViewModels;

public sealed record BookReviewItemViewModel(
    string DisplayName,
    int Rating,
    string Content,
    DateTimeOffset CreatedAt);
