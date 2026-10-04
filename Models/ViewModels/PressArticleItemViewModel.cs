namespace Nha_Gia_Kim.Models.ViewModels;

public sealed record PressArticleItemViewModel(
    string PressName,
    string ArticleTitle,
    string? ArticleUrl,
    string? Snippet);
