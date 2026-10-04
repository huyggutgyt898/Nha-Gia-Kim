using System.ComponentModel.DataAnnotations;

namespace Nha_Gia_Kim.Models.ViewModels;

public sealed class BookReviewSubmission
{
    [StringLength(80, ErrorMessage = "Tên hiển thị không được vượt quá 80 ký tự.")]
    [Display(Name = "Tên hiển thị")]
    public string? DisplayName { get; set; }

    [Range(1, 5, ErrorMessage = "Vui lòng chọn số sao từ 1 đến 5.")]
    [Display(Name = "Số sao")]
    public int Rating { get; set; }

    [Required(ErrorMessage = "Vui lòng chia sẻ cảm nhận của bạn.")]
    [StringLength(1000, MinimumLength = 8, ErrorMessage = "Nội dung cần từ 8 đến 1.000 ký tự.")]
    [Display(Name = "Cảm nhận")]
    public string Content { get; set; } = string.Empty;
}
