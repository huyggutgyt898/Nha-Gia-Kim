using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using Nha_Gia_Kim.Data;
using Nha_Gia_Kim.Models.Api;
using Nha_Gia_Kim.Models.Entities;

namespace Nha_Gia_Kim.Services;

public sealed class CartService(BookstoreDbContext dbContext) : ICartService
{
    public async Task<CartResponse> GetAsync(Guid cartId, CancellationToken cancellationToken)
    {
        var cart = await GetCartAsync(cartId, cancellationToken)
            ?? throw new ResourceNotFoundException("Không tìm thấy giỏ hàng.");
        return ToResponse(cart);
    }

    public async Task<CartResponse> AddItemAsync(
        Guid cartId,
        AddCartItemRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            return await AddItemCoreAsync(cartId, request, cancellationToken);
        }
        catch (DbUpdateException exception) when (IsUniqueConstraintViolation(exception))
        {
            dbContext.ChangeTracker.Clear();
            return await AddItemCoreAsync(cartId, request, cancellationToken);
        }
    }

    private async Task<CartResponse> AddItemCoreAsync(
        Guid cartId,
        AddCartItemRequest request,
        CancellationToken cancellationToken)
    {
        EnsureCartId(cartId);

        var book = await dbContext.Books
            .Include(item => item.Author)
            .SingleOrDefaultAsync(
            item => item.Id == request.BookId && item.IsActive,
            cancellationToken)
            ?? throw new ResourceNotFoundException("Không tìm thấy sách đang được bán.");

        var cart = await GetCartAsync(cartId, cancellationToken);
        if (cart is null)
        {
            cart = new Cart
            {
                Id = cartId,
                CreatedAt = DateTimeOffset.UtcNow,
                UpdatedAt = DateTimeOffset.UtcNow
            };
            dbContext.Carts.Add(cart);
        }

        var item = cart.Items.SingleOrDefault(cartItem => cartItem.BookId == book.Id);
        var newQuantity = (item?.Quantity ?? 0) + request.Quantity;
        ValidateQuantity(newQuantity, book.StockQuantity);

        if (item is null)
        {
            cart.Items.Add(new CartItem { CartId = cartId, BookId = book.Id, Book = book, Quantity = newQuantity });
        }
        else
        {
            item.Quantity = newQuantity;
        }

        cart.UpdatedAt = DateTimeOffset.UtcNow;
        await dbContext.SaveChangesAsync(cancellationToken);
        return ToResponse(cart);
    }

    private static bool IsUniqueConstraintViolation(DbUpdateException exception) =>
        exception.InnerException is SqlException { Number: 2601 or 2627 };

    public async Task<CartResponse> UpdateItemAsync(
        Guid cartId,
        int bookId,
        UpdateCartItemRequest request,
        CancellationToken cancellationToken)
    {
        var cart = await GetCartAsync(cartId, cancellationToken)
            ?? throw new ResourceNotFoundException("Không tìm thấy giỏ hàng.");
        var item = cart.Items.SingleOrDefault(cartItem => cartItem.BookId == bookId)
            ?? throw new ResourceNotFoundException("Sách không có trong giỏ hàng.");
        ValidateQuantity(request.Quantity, item.Book.StockQuantity);
        item.Quantity = request.Quantity;
        cart.UpdatedAt = DateTimeOffset.UtcNow;
        await dbContext.SaveChangesAsync(cancellationToken);
        return ToResponse(cart);
    }

    public async Task RemoveItemAsync(Guid cartId, int bookId, CancellationToken cancellationToken)
    {
        var cart = await GetCartAsync(cartId, cancellationToken)
            ?? throw new ResourceNotFoundException("Không tìm thấy giỏ hàng.");
        var item = cart.Items.SingleOrDefault(cartItem => cartItem.BookId == bookId)
            ?? throw new ResourceNotFoundException("Sách không có trong giỏ hàng.");
        dbContext.CartItems.Remove(item);
        cart.UpdatedAt = DateTimeOffset.UtcNow;
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private Task<Cart?> GetCartAsync(Guid cartId, CancellationToken cancellationToken) =>
        dbContext.Carts
            .Include(cart => cart.Items)
            .ThenInclude(item => item.Book)
            .ThenInclude(book => book.Author)
            .SingleOrDefaultAsync(cart => cart.Id == cartId, cancellationToken);

    private static void EnsureCartId(Guid cartId)
    {
        if (cartId == Guid.Empty)
        {
            throw new InvalidOperationRequestException("CartId phải là một GUID hợp lệ.");
        }
    }

    private static void ValidateQuantity(int quantity, int availableStock)
    {
        if (quantity > 100)
        {
            throw new InvalidOperationRequestException("Mỗi mặt hàng chỉ được đặt tối đa 100 cuốn.");
        }

        if (quantity > availableStock)
        {
            throw new BusinessConflictException("Số lượng sách trong kho không đủ.");
        }
    }

    private static CartResponse ToResponse(Cart cart)
    {
        var items = cart.Items
            .OrderBy(item => item.Book.Title)
            .Select(item => new CartItemResponse(
                item.BookId,
                item.Book.Title,
                item.Book.Author.Name,
                item.Book.CoverImageUrl,
                item.Book.Price,
                item.Quantity,
                item.Book.Price * item.Quantity,
                item.Book.StockQuantity))
            .ToArray();

        return new CartResponse(cart.Id, items, items.Sum(item => item.LineTotal));
    }
}
