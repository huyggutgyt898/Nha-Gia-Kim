using Microsoft.EntityFrameworkCore;
using Nha_Gia_Kim.Data;
using Nha_Gia_Kim.Models.Api;
using Nha_Gia_Kim.Models.Entities;

namespace Nha_Gia_Kim.Services;

public sealed class OrderService(BookstoreDbContext dbContext) : IOrderService
{
    public async Task<OrderResponse?> GetByNumberAsync(Guid orderNumber, CancellationToken cancellationToken)
    {
        var order = await dbContext.Orders
            .AsNoTracking()
            .Include(item => item.Items)
            .SingleOrDefaultAsync(item => item.OrderNumber == orderNumber, cancellationToken);

        return order is null ? null : ToResponse(order);
    }

    public async Task<OrderResponse> CreateAsync(CreateOrderRequest request, CancellationToken cancellationToken)
    {
        if (request.CartId == Guid.Empty)
        {
            throw new InvalidOperationRequestException("CartId phải là một GUID hợp lệ.");
        }

        var cart = await dbContext.Carts
            .Include(item => item.Items)
            .ThenInclude(item => item.Book)
            .SingleOrDefaultAsync(item => item.Id == request.CartId, cancellationToken)
            ?? throw new ResourceNotFoundException("Không tìm thấy giỏ hàng.");

        if (cart.Items.Count == 0)
        {
            throw new InvalidOperationRequestException("Không thể đặt hàng với giỏ hàng trống.");
        }

        foreach (var cartItem in cart.Items)
        {
            if (!cartItem.Book.IsActive)
            {
                throw new BusinessConflictException($"Sách “{cartItem.Book.Title}” hiện không còn được bán.");
            }

            if (cartItem.Quantity > cartItem.Book.StockQuantity)
            {
                throw new BusinessConflictException($"Số lượng sách “{cartItem.Book.Title}” trong kho không đủ.");
            }
        }

        var now = DateTimeOffset.UtcNow;
        var order = new CustomerOrder
        {
            OrderNumber = Guid.NewGuid(),
            CustomerName = request.CustomerName.Trim(),
            CustomerEmail = request.CustomerEmail.Trim(),
            CustomerPhone = request.CustomerPhone.Trim(),
            ShippingAddress = request.ShippingAddress.Trim(),
            ProvinceCity = request.ProvinceCity.Trim(),
            PaymentMethod = request.PaymentMethod,
            Status = OrderStatuses.Registered,
            CreatedAt = now
        };

        foreach (var cartItem in cart.Items)
        {
            cartItem.Book.StockQuantity -= cartItem.Quantity;
            order.Items.Add(new OrderItem
            {
                BookId = cartItem.BookId,
                BookTitle = cartItem.Book.Title,
                Quantity = cartItem.Quantity,
                UnitPrice = cartItem.Book.Price
            });
            order.TotalAmount += cartItem.Book.Price * cartItem.Quantity;
        }

        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
        dbContext.Orders.Add(order);
        dbContext.Carts.Remove(cart);

        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw new BusinessConflictException("Kho sách vừa thay đổi. Hãy tải lại giỏ hàng và thử lại.");
        }

        return ToResponse(order);
    }

    private static OrderResponse ToResponse(CustomerOrder order) =>
        new(
            order.OrderNumber,
            order.Status,
            order.TotalAmount,
            order.CreatedAt,
            order.Items
                .Select(item => new OrderItemResponse(
                    item.BookId,
                    item.BookTitle,
                    item.Quantity,
                    item.UnitPrice,
                    item.UnitPrice * item.Quantity))
                .ToArray());
}
