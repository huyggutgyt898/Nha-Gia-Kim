using Nha_Gia_Kim.Models.ViewModels;

namespace Nha_Gia_Kim.Services;

public interface IAdminOrderService
{
    Task<AdminOrdersViewModel> GetOrdersAsync(
        string? search,
        string? status,
        int page,
        CancellationToken cancellationToken);

    Task<AdminOrderDetailViewModel?> GetOrderDetailsAsync(
        int orderId,
        CancellationToken cancellationToken);

    Task<AdminSaveResult> UpdateStatusAsync(
        int orderId,
        string status,
        CancellationToken cancellationToken);
}
