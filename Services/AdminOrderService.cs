using Microsoft.EntityFrameworkCore;
using Nha_Gia_Kim.Data;
using Nha_Gia_Kim.Models.Entities;
using Nha_Gia_Kim.Models.ViewModels;

namespace Nha_Gia_Kim.Services;

public sealed class AdminOrderService(BookstoreDbContext dbContext) : IAdminOrderService
{
    private const int PageSize = 25;

    public async Task<AdminOrdersViewModel> GetOrdersAsync(
        string? search,
        string? status,
        int page,
        CancellationToken cancellationToken)
    {
        var countsByStatus = await dbContext.Orders
            .AsNoTracking()
            .GroupBy(order => order.Status)
            .Select(group => new { Status = group.Key, Count = group.Count() })
            .ToDictionaryAsync(item => item.Status, item => item.Count, cancellationToken);
        var statusCounts = OrderStatuses.All.ToDictionary(
            item => item,
            item => countsByStatus.GetValueOrDefault(item));

        var query = dbContext.Orders.AsNoTracking();
        var normalizedStatus = OrderStatuses.Contains(status) ? status : null;
        if (normalizedStatus is not null)
        {
            query = query.Where(order => order.Status == normalizedStatus);
        }

        var normalizedSearch = search?.Trim();
        if (!string.IsNullOrWhiteSpace(normalizedSearch))
        {
            var code = normalizedSearch
                .TrimStart('#')
                .Replace("NGK-", string.Empty, StringComparison.OrdinalIgnoreCase);
            var matchesId = int.TryParse(code, out var orderId);
            var matchesNumber = Guid.TryParse(normalizedSearch, out var orderNumber);

            query = query.Where(order =>
                order.CustomerName.Contains(normalizedSearch)
                || order.CustomerEmail.Contains(normalizedSearch)
                || order.CustomerPhone.Contains(normalizedSearch)
                || (matchesId && order.Id == orderId)
                || (matchesNumber && order.OrderNumber == orderNumber));
        }

        var totalOrders = await query.CountAsync(cancellationToken);
        var totalPages = Math.Max(1, (int)Math.Ceiling(totalOrders / (double)PageSize));
        var currentPage = Math.Clamp(page, 1, totalPages);
        var orders = await query
            .OrderByDescending(order => order.CreatedAt)
            .ThenByDescending(order => order.Id)
            .Skip((currentPage - 1) * PageSize)
            .Take(PageSize)
            .Select(order => new AdminOrderRowViewModel(
                order.Id,
                $"NGK-{order.Id:D6}",
                order.CustomerName,
                order.CustomerEmail,
                order.CustomerPhone,
                order.CreatedAt,
                order.PaymentMethod,
                order.TotalAmount,
                order.Items.Sum(item => item.Quantity),
                order.Status))
            .ToArrayAsync(cancellationToken);

        return new AdminOrdersViewModel(
            orders,
            statusCounts,
            normalizedSearch,
            normalizedStatus,
            totalOrders,
            currentPage,
            totalPages);
    }

    public async Task<AdminSaveResult> UpdateStatusAsync(
        int orderId,
        string status,
        CancellationToken cancellationToken)
    {
        if (!OrderStatuses.Contains(status))
        {
            return AdminSaveResult.InvalidInput;
        }

        var order = await dbContext.Orders
            .SingleOrDefaultAsync(item => item.Id == orderId, cancellationToken);
        if (order is null)
        {
            return AdminSaveResult.NotFound;
        }

        order.Status = status;
        await dbContext.SaveChangesAsync(cancellationToken);
        return AdminSaveResult.Success;
    }

    public async Task<AdminOrderDetailViewModel?> GetOrderDetailsAsync(
        int orderId,
        CancellationToken cancellationToken)
    {
        return await dbContext.Orders
            .AsNoTracking()
            .Where(order => order.Id == orderId)
            .Select(order => new AdminOrderDetailViewModel(
                order.Id,
                $"NGK-{order.Id:D6}",
                order.CustomerName,
                order.CustomerEmail,
                order.CustomerPhone,
                order.ShippingAddress,
                order.ProvinceCity,
                order.PaymentMethod,
                order.CreatedAt,
                order.Status,
                order.TotalAmount,
                order.Items
                    .OrderBy(item => item.Id)
                    .Select(item => new AdminOrderItemViewModel(
                        item.BookTitle,
                        item.Quantity,
                        item.UnitPrice,
                        item.UnitPrice * item.Quantity))
                    .ToArray()))
            .SingleOrDefaultAsync(cancellationToken);
    }
}
