using Nha_Gia_Kim.Models.ViewModels;

namespace Nha_Gia_Kim.Services;

public interface IAdminManagementService
{
    Task<AdminBooksViewModel> GetBooksAsync(string? search, CancellationToken cancellationToken);
    Task<AdminBookFormViewModel?> GetBookFormAsync(int? id, CancellationToken cancellationToken);
    Task PopulateBookOptionsAsync(AdminBookFormViewModel model, CancellationToken cancellationToken);
    Task<AdminSaveResult> SaveBookAsync(int? id, AdminBookFormViewModel model, CancellationToken cancellationToken);
    Task<AdminSaveResult> ToggleBookAvailabilityAsync(int id, CancellationToken cancellationToken);
    Task<AdminSaveResult> DeleteBookAsync(int id, CancellationToken cancellationToken);

    Task<AdminContentViewModel> GetContentAsync(string? tab, CancellationToken cancellationToken);
    Task<AdminAuthorFormViewModel?> GetAuthorFormAsync(int? id, CancellationToken cancellationToken);
    Task<AdminSaveResult> SaveAuthorAsync(int? id, AdminAuthorFormViewModel model, CancellationToken cancellationToken);
    Task<AdminSaveResult> DeleteAuthorAsync(int id, CancellationToken cancellationToken);
    Task<AdminCategoryFormViewModel?> GetCategoryFormAsync(int? id, CancellationToken cancellationToken);
    Task<AdminSaveResult> SaveCategoryAsync(int? id, AdminCategoryFormViewModel model, CancellationToken cancellationToken);
    Task<AdminSaveResult> DeleteCategoryAsync(int id, CancellationToken cancellationToken);
    Task<AdminArticleFormViewModel?> GetArticleFormAsync(int? id, CancellationToken cancellationToken);
    Task PopulateArticleBooksAsync(AdminArticleFormViewModel model, CancellationToken cancellationToken);
    Task<AdminSaveResult> SaveArticleAsync(int? id, AdminArticleFormViewModel model, CancellationToken cancellationToken);
    Task<AdminSaveResult> DeleteArticleAsync(int id, CancellationToken cancellationToken);

    Task<AdminFeedbackViewModel> GetFeedbackAsync(string? search, CancellationToken cancellationToken);
    Task<AdminSaveResult> DeleteFeedbackAsync(int id, CancellationToken cancellationToken);
}
