using Nha_Gia_Kim.Models.Api;

namespace Nha_Gia_Kim.Models.ViewModels;

public sealed record HomeIndexViewModel(
    BookListResponse Books,
    IReadOnlyList<CategoryResponse> Categories,
    string? Search,
    int? SelectedCategoryId);
