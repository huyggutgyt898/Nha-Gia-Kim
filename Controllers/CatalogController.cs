using Microsoft.AspNetCore.Mvc;
using Nha_Gia_Kim.Models.ViewModels;
using Nha_Gia_Kim.Services;

namespace Nha_Gia_Kim.Controllers;

public sealed class CatalogController(ICatalogService catalogService) : Controller
{
    public async Task<IActionResult> Index(
        string? search,
        int? categoryId,
        CancellationToken cancellationToken)
    {
        var books = await catalogService.GetBooksAsync(search, categoryId, 1, 100, cancellationToken);
        var categories = await catalogService.GetCategoriesAsync(cancellationToken);

        return View(new HomeIndexViewModel(books, categories, search, categoryId));
    }
}
