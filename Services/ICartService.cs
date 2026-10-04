using Nha_Gia_Kim.Models.Api;

namespace Nha_Gia_Kim.Services;

public interface ICartService
{
    Task<CartResponse> GetAsync(Guid cartId, CancellationToken cancellationToken);
    Task<CartResponse> AddItemAsync(Guid cartId, AddCartItemRequest request, CancellationToken cancellationToken);
    Task<CartResponse> UpdateItemAsync(Guid cartId, int bookId, UpdateCartItemRequest request, CancellationToken cancellationToken);
    Task RemoveItemAsync(Guid cartId, int bookId, CancellationToken cancellationToken);
}
