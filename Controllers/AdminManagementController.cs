using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nha_Gia_Kim.Models.ViewModels;
using Nha_Gia_Kim.Services;

namespace Nha_Gia_Kim.Controllers;

[Route("admin")]
[Authorize(Policy = "AdminOnly")]
public sealed class AdminManagementController(IAdminManagementService managementService) : Controller
{
    [HttpGet("books")]
    public async Task<IActionResult> Books(string? search, CancellationToken cancellationToken)
    {
        SetPage("books", "Sách");
        var model = await managementService.GetBooksAsync(search, cancellationToken);
        return View("Books", model);
    }

    [HttpGet("books/new")]
    public async Task<IActionResult> NewBook(CancellationToken cancellationToken)
    {
        SetPage("books", "Thêm sách");
        var model = await managementService.GetBookFormAsync(null, cancellationToken);
        return View("BookForm", model);
    }

    [HttpPost("books/new")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> NewBook(
        AdminBookFormViewModel model,
        CancellationToken cancellationToken)
    {
        if (ModelState.IsValid)
        {
            var result = await managementService.SaveBookAsync(null, model, cancellationToken);
            if (result == AdminSaveResult.Success)
            {
                TempData["AdminNotice"] = "Đã thêm sách.";
                return RedirectToAction(nameof(Books));
            }

            AddSaveError(result, "CoverImageUrl");
        }

        await managementService.PopulateBookOptionsAsync(model, cancellationToken);
        SetPage("books", "Thêm sách");
        return View("BookForm", model);
    }

    [HttpGet("books/{id:int}/edit")]
    public async Task<IActionResult> EditBook(int id, CancellationToken cancellationToken)
    {
        var model = await managementService.GetBookFormAsync(id, cancellationToken);
        if (model is null)
        {
            return NotFound();
        }

        SetPage("books", "Sửa thông tin sách");
        return View("BookForm", model);
    }

    [HttpPost("books/{id:int}/edit")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditBook(
        int id,
        AdminBookFormViewModel model,
        CancellationToken cancellationToken)
    {
        if (ModelState.IsValid)
        {
            var result = await managementService.SaveBookAsync(id, model, cancellationToken);
            if (result == AdminSaveResult.Success)
            {
                TempData["AdminNotice"] = "Đã lưu thông tin sách.";
                return RedirectToAction(nameof(Books));
            }

            if (result == AdminSaveResult.NotFound)
            {
                return NotFound();
            }

            AddSaveError(result, "CoverImageUrl");
        }

        model.Id = id;
        await managementService.PopulateBookOptionsAsync(model, cancellationToken);
        SetPage("books", "Sửa thông tin sách");
        return View("BookForm", model);
    }

    [HttpPost("books/{id:int}/availability")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleBookAvailability(int id, CancellationToken cancellationToken)
    {
        var result = await managementService.ToggleBookAvailabilityAsync(id, cancellationToken);
        TempData["AdminNotice"] = result == AdminSaveResult.Success
            ? "Đã cập nhật trạng thái bán của sách."
            : "Không tìm thấy sách.";
        return RedirectToAction(nameof(Books));
    }

    [HttpGet("content")]
    public async Task<IActionResult> Content(string? tab, CancellationToken cancellationToken)
    {
        SetPage("content", "Nội dung sách");
        var model = await managementService.GetContentAsync(tab, cancellationToken);
        return View(model);
    }

    [HttpGet("authors/new")]
    public IActionResult NewAuthor()
    {
        SetPage("content", "Thêm tác giả");
        return View("AuthorForm", new AdminAuthorFormViewModel());
    }

    [HttpPost("authors/new")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> NewAuthor(
        AdminAuthorFormViewModel model,
        CancellationToken cancellationToken)
    {
        if (ModelState.IsValid)
        {
            var result = await managementService.SaveAuthorAsync(null, model, cancellationToken);
            if (result == AdminSaveResult.Success)
            {
                TempData["AdminNotice"] = "Đã thêm tác giả.";
                return RedirectToAction(nameof(Content), new { tab = "authors" });
            }

            AddSaveError(result, "AvatarUrl");
        }

        SetPage("content", "Thêm tác giả");
        return View("AuthorForm", model);
    }

    [HttpGet("authors/{id:int}/edit")]
    public async Task<IActionResult> EditAuthor(int id, CancellationToken cancellationToken)
    {
        var model = await managementService.GetAuthorFormAsync(id, cancellationToken);
        if (model is null)
        {
            return NotFound();
        }

        SetPage("content", "Sửa tác giả");
        return View("AuthorForm", model);
    }

    [HttpPost("authors/{id:int}/edit")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditAuthor(
        int id,
        AdminAuthorFormViewModel model,
        CancellationToken cancellationToken)
    {
        if (ModelState.IsValid)
        {
            var result = await managementService.SaveAuthorAsync(id, model, cancellationToken);
            if (result == AdminSaveResult.Success)
            {
                TempData["AdminNotice"] = "Đã lưu tác giả.";
                return RedirectToAction(nameof(Content), new { tab = "authors" });
            }

            if (result == AdminSaveResult.NotFound)
            {
                return NotFound();
            }

            AddSaveError(result, "AvatarUrl");
        }

        model.Id = id;
        SetPage("content", "Sửa tác giả");
        return View("AuthorForm", model);
    }

    [HttpPost("authors/{id:int}/delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteAuthor(int id, CancellationToken cancellationToken)
    {
        var result = await managementService.DeleteAuthorAsync(id, cancellationToken);
        TempData["AdminNotice"] = result switch
        {
            AdminSaveResult.Success => "Đã xóa tác giả.",
            AdminSaveResult.InUse => "Không thể xóa tác giả đang được sách sử dụng.",
            _ => "Không tìm thấy tác giả."
        };
        return RedirectToAction(nameof(Content), new { tab = "authors" });
    }

    [HttpGet("categories/new")]
    public IActionResult NewCategory()
    {
        SetPage("content", "Thêm thể loại");
        return View("CategoryForm", new AdminCategoryFormViewModel());
    }

    [HttpPost("categories/new")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> NewCategory(
        AdminCategoryFormViewModel model,
        CancellationToken cancellationToken)
    {
        if (ModelState.IsValid)
        {
            var result = await managementService.SaveCategoryAsync(null, model, cancellationToken);
            if (result == AdminSaveResult.Success)
            {
                TempData["AdminNotice"] = "Đã thêm thể loại.";
                return RedirectToAction(nameof(Content), new { tab = "categories" });
            }

            AddSaveError(result);
        }

        SetPage("content", "Thêm thể loại");
        return View("CategoryForm", model);
    }

    [HttpGet("categories/{id:int}/edit")]
    public async Task<IActionResult> EditCategory(int id, CancellationToken cancellationToken)
    {
        var model = await managementService.GetCategoryFormAsync(id, cancellationToken);
        if (model is null)
        {
            return NotFound();
        }

        SetPage("content", "Sửa thể loại");
        return View("CategoryForm", model);
    }

    [HttpPost("categories/{id:int}/edit")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditCategory(
        int id,
        AdminCategoryFormViewModel model,
        CancellationToken cancellationToken)
    {
        if (ModelState.IsValid)
        {
            var result = await managementService.SaveCategoryAsync(id, model, cancellationToken);
            if (result == AdminSaveResult.Success)
            {
                TempData["AdminNotice"] = "Đã lưu thể loại.";
                return RedirectToAction(nameof(Content), new { tab = "categories" });
            }

            if (result == AdminSaveResult.NotFound)
            {
                return NotFound();
            }

            AddSaveError(result);
        }

        model.Id = id;
        SetPage("content", "Sửa thể loại");
        return View("CategoryForm", model);
    }

    [HttpPost("categories/{id:int}/delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteCategory(int id, CancellationToken cancellationToken)
    {
        var result = await managementService.DeleteCategoryAsync(id, cancellationToken);
        TempData["AdminNotice"] = result switch
        {
            AdminSaveResult.Success => "Đã xóa thể loại.",
            AdminSaveResult.InUse => "Không thể xóa thể loại đang có sách.",
            _ => "Không tìm thấy thể loại."
        };
        return RedirectToAction(nameof(Content), new { tab = "categories" });
    }

    [HttpGet("articles/new")]
    public async Task<IActionResult> NewArticle(CancellationToken cancellationToken)
    {
        SetPage("content", "Thêm bài báo");
        var model = await managementService.GetArticleFormAsync(null, cancellationToken);
        return View("ArticleForm", model);
    }

    [HttpPost("articles/new")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> NewArticle(
        AdminArticleFormViewModel model,
        CancellationToken cancellationToken)
    {
        if (ModelState.IsValid)
        {
            var result = await managementService.SaveArticleAsync(null, model, cancellationToken);
            if (result == AdminSaveResult.Success)
            {
                TempData["AdminNotice"] = "Đã thêm bài báo.";
                return RedirectToAction(nameof(Content), new { tab = "articles" });
            }

            AddSaveError(result, "ArticleUrl");
        }

        await managementService.PopulateArticleBooksAsync(model, cancellationToken);
        SetPage("content", "Thêm bài báo");
        return View("ArticleForm", model);
    }

    [HttpGet("articles/{id:int}/edit")]
    public async Task<IActionResult> EditArticle(int id, CancellationToken cancellationToken)
    {
        var model = await managementService.GetArticleFormAsync(id, cancellationToken);
        if (model is null)
        {
            return NotFound();
        }

        SetPage("content", "Sửa bài báo");
        return View("ArticleForm", model);
    }

    [HttpPost("articles/{id:int}/edit")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditArticle(
        int id,
        AdminArticleFormViewModel model,
        CancellationToken cancellationToken)
    {
        if (ModelState.IsValid)
        {
            var result = await managementService.SaveArticleAsync(id, model, cancellationToken);
            if (result == AdminSaveResult.Success)
            {
                TempData["AdminNotice"] = "Đã lưu bài báo.";
                return RedirectToAction(nameof(Content), new { tab = "articles" });
            }

            if (result == AdminSaveResult.NotFound)
            {
                return NotFound();
            }

            AddSaveError(result, "ArticleUrl");
        }

        model.Id = id;
        await managementService.PopulateArticleBooksAsync(model, cancellationToken);
        SetPage("content", "Sửa bài báo");
        return View("ArticleForm", model);
    }

    [HttpPost("articles/{id:int}/delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteArticle(int id, CancellationToken cancellationToken)
    {
        var result = await managementService.DeleteArticleAsync(id, cancellationToken);
        TempData["AdminNotice"] = result == AdminSaveResult.Success
            ? "Đã xóa bài báo."
            : "Không tìm thấy bài báo.";
        return RedirectToAction(nameof(Content), new { tab = "articles" });
    }

    [HttpPost("feedback/{id:int}/delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteFeedback(
        int id,
        string? search,
        CancellationToken cancellationToken)
    {
        var result = await managementService.DeleteFeedbackAsync(id, cancellationToken);
        TempData["AdminNotice"] = result == AdminSaveResult.Success
            ? "Đã xóa phản hồi."
            : "Không tìm thấy phản hồi.";
        return RedirectToAction("Feedback", "Admin", new { search });
    }

    private void SetPage(string section, string title)
    {
        Response.Headers.CacheControl = "no-store";
        ViewData["Title"] = title;
        ViewData["AdminSection"] = section;
        ViewData["ShowAdminShell"] = true;
    }

    private void AddSaveError(AdminSaveResult result, string? field = null)
    {
        var message = result switch
        {
            AdminSaveResult.Conflict => "Dữ liệu đã thay đổi hoặc bị trùng. Tải lại rồi thử lại.",
            AdminSaveResult.InvalidReference => "Chọn một mục có trong danh sách.",
            AdminSaveResult.InvalidInput => "Đường dẫn chỉ được là đường dẫn nội bộ hoặc địa chỉ HTTP(S).",
            _ => "Không thể lưu thay đổi."
        };
        ModelState.AddModelError(field ?? string.Empty, message);
    }
}
