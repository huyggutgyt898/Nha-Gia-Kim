using System.ComponentModel.DataAnnotations;

namespace Nha_Gia_Kim.Models.ViewModels;

public sealed class AdminLoginViewModel
{
    [Required(ErrorMessage = "Nhập tên đăng nhập.")]
    [StringLength(100)]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Nhập mật khẩu.")]
    [StringLength(200)]
    public string Password { get; set; } = string.Empty;

    public string? ReturnUrl { get; set; }
}

public sealed record AdminOptionViewModel(int Id, string Name);

public sealed record AdminOrderRowViewModel(
    int Id,
    string OrderCode,
    string CustomerName,
    string CustomerEmail,
    string CustomerPhone,
    DateTimeOffset CreatedAt,
    string PaymentMethod,
    decimal TotalAmount,
    int TotalCopies,
    string Status);

public sealed record AdminOrdersViewModel(
    IReadOnlyList<AdminOrderRowViewModel> Orders,
    IReadOnlyDictionary<string, int> StatusCounts,
    string? Search,
    string? Status,
    int TotalOrders,
    int Page,
    int TotalPages);

public sealed record AdminOrderItemViewModel(
    string BookTitle,
    int Quantity,
    decimal UnitPrice,
    decimal LineTotal);

public sealed record AdminOrderDetailViewModel(
    int Id,
    string OrderCode,
    string CustomerName,
    string CustomerEmail,
    string CustomerPhone,
    string ShippingAddress,
    string ProvinceCity,
    string PaymentMethod,
    DateTimeOffset CreatedAt,
    string Status,
    decimal TotalAmount,
    IReadOnlyList<AdminOrderItemViewModel> Items);

public sealed record AdminBookRowViewModel(
    int Id,
    string Title,
    string AuthorName,
    string CategoryName,
    decimal Price,
    int StockQuantity,
    bool IsActive,
    string? CoverImageUrl);

public sealed record AdminBooksViewModel(
    IReadOnlyList<AdminBookRowViewModel> Books,
    string? Search,
    int TotalBooks);

public sealed class AdminBookFormViewModel
{
    public int? Id { get; set; }

    [Required(ErrorMessage = "Nhập tên sách.")]
    [StringLength(255)]
    public string Title { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Subtitle { get; set; }

    [Required(ErrorMessage = "Chọn tác giả.")]
    [Range(1, int.MaxValue, ErrorMessage = "Chọn tác giả hợp lệ.")]
    public int AuthorId { get; set; }

    [Required(ErrorMessage = "Chọn thể loại.")]
    [Range(1, int.MaxValue, ErrorMessage = "Chọn thể loại hợp lệ.")]
    public int CategoryId { get; set; }

    [Range(0, 99_999_999.99, ErrorMessage = "Giá bán không hợp lệ.")]
    public decimal Price { get; set; }

    [Range(0, 99_999_999.99, ErrorMessage = "Giá gốc không hợp lệ.")]
    public decimal? OriginalPrice { get; set; }

    [StringLength(20)]
    public string? Isbn { get; set; }

    [StringLength(500)]
    public string? CoverImageUrl { get; set; }

    [StringLength(500)]
    public string? FilePreviewUrl { get; set; }

    public string? Description { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Tồn kho không hợp lệ.")]
    public int StockQuantity { get; set; }

    public bool IsActive { get; set; } = true;

    public string? RowVersion { get; set; }

    public IReadOnlyList<AdminOptionViewModel> Authors { get; set; } = [];
    public IReadOnlyList<AdminOptionViewModel> Categories { get; set; } = [];
}

public sealed record AdminAuthorRowViewModel(
    int Id,
    string Name,
    string? Bio,
    string? AvatarUrl,
    int BookCount);

public sealed record AdminCategoryRowViewModel(
    int Id,
    string Name,
    string Slug,
    int BookCount);

public sealed record AdminArticleRowViewModel(
    int Id,
    string PressName,
    string ArticleTitle,
    string? ArticleUrl,
    string BookTitle);

public sealed record AdminContentViewModel(
    string Tab,
    IReadOnlyList<AdminArticleRowViewModel> Articles,
    IReadOnlyList<AdminAuthorRowViewModel> Authors,
    IReadOnlyList<AdminCategoryRowViewModel> Categories);

public sealed class AdminAuthorFormViewModel
{
    public int? Id { get; set; }

    [Required(ErrorMessage = "Nhập tên tác giả.")]
    [StringLength(255)]
    public string Name { get; set; } = string.Empty;

    public string? Bio { get; set; }

    [StringLength(500)]
    public string? AvatarUrl { get; set; }
}

public sealed class AdminCategoryFormViewModel
{
    public int? Id { get; set; }

    [Required(ErrorMessage = "Nhập tên thể loại.")]
    [StringLength(255)]
    public string Name { get; set; } = string.Empty;
}

public sealed class AdminArticleFormViewModel
{
    public int? Id { get; set; }

    [Required(ErrorMessage = "Chọn sách liên quan.")]
    [Range(1, int.MaxValue, ErrorMessage = "Chọn sách hợp lệ.")]
    public int BookId { get; set; }

    [Required(ErrorMessage = "Nhập tên đơn vị báo chí.")]
    [StringLength(255)]
    public string PressName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Nhập tiêu đề bài báo.")]
    [StringLength(500)]
    public string ArticleTitle { get; set; } = string.Empty;

    [StringLength(500)]
    public string? ArticleUrl { get; set; }

    public string? Snippet { get; set; }

    public IReadOnlyList<AdminOptionViewModel> Books { get; set; } = [];
}

public sealed record AdminFeedbackRowViewModel(
    int Id,
    string CustomerName,
    int Rating,
    string? Comment,
    DateTimeOffset CreatedAt,
    string BookTitle);

public sealed record AdminFeedbackViewModel(
    IReadOnlyList<AdminFeedbackRowViewModel> Feedbacks,
    string? Search,
    int TotalFeedbacks);

public enum AdminSaveResult
{
    Success,
    NotFound,
    Conflict,
    InvalidReference,
    InvalidInput,
    InUse
}
