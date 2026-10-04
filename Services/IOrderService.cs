using Nha_Gia_Kim.Models.Api;

namespace Nha_Gia_Kim.Services;

public interface IOrderService
{
    Task<OrderResponse> CreateAsync(CreateOrderRequest request, CancellationToken cancellationToken);
    Task<OrderResponse?> GetByNumberAsync(Guid orderNumber, CancellationToken cancellationToken);
}
