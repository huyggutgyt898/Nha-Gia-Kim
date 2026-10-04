using Nha_Gia_Kim.Models.ViewModels;

namespace Nha_Gia_Kim.Services;

public interface IPressArticleService
{
    Task<IReadOnlyList<PressArticleItemViewModel>> GetForBookAsync(
        int bookId,
        CancellationToken cancellationToken);
}
