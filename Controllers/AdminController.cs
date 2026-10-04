using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Nha_Gia_Kim.Models.ViewModels;
using Nha_Gia_Kim.Services;

namespace Nha_Gia_Kim.Controllers;

[Route("admin")]
[Authorize(Policy = "AdminOnly")]
public sealed class AdminController(
    IAdminAuthenticationService authenticationService,
    IAdminOrderService orderService,
    IAdminManagementService managementService) : Controller
{
    [AllowAnonymous]
    [HttpGet("login")]
    public IActionResult Login(string? returnUrl, bool loginThrottled = false)
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction(nameof(Orders));
        }

        Response.Headers.CacheControl = "no-store";
        if (loginThrottled)
        {
            Response.StatusCode = StatusCodes.Status429TooManyRequests;
            Response.Headers.RetryAfter = "900";
            ModelState.AddModelError(
                string.Empty,
                "Bạn đã thử đăng nhập quá nhiều lần. Hãy chờ 15 phút trước khi thử lại.");
        }

        ViewData["Title"] = "Đăng nhập quản trị";
        return View(new AdminLoginViewModel { ReturnUrl = returnUrl });
    }

    [AllowAnonymous]
    [HttpPost("login")]
    [ValidateAntiForgeryToken]
    [EnableRateLimiting("admin-login")]
    public async Task<IActionResult> Login(
        AdminLoginViewModel model,
        CancellationToken cancellationToken)
    {
        Response.Headers.CacheControl = "no-store";
        ViewData["Title"] = "Đăng nhập quản trị";
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var principal = await authenticationService.AuthenticateAsync(
            model.Username,
            model.Password,
            cancellationToken);
        if (principal is null)
        {
            ModelState.AddModelError(string.Empty, "Tên đăng nhập hoặc mật khẩu không đúng.");
            return View(model);
        }

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal,
            new AuthenticationProperties { IsPersistent = false });

        return Url.IsLocalUrl(model.ReturnUrl)
            ? LocalRedirect(model.ReturnUrl!)
            : RedirectToAction(nameof(Orders));
    }

    [HttpPost("logout")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction(nameof(Login));
    }

    [HttpGet("orders")]
    public async Task<IActionResult> Orders(
        string? search,
        string? status,
        int page = 1,
        CancellationToken cancellationToken = default)
    {
        Response.Headers.CacheControl = "no-store";
        ViewData["Title"] = "Đơn hàng";
        ViewData["AdminSection"] = "orders";
        ViewData["ShowAdminShell"] = true;
        var model = await orderService.GetOrdersAsync(search, status, page, cancellationToken);
        return View(model);
    }

    [HttpGet("orders/{id:int}")]
    public async Task<IActionResult> OrderDetails(int id, CancellationToken cancellationToken)
    {
        Response.Headers.CacheControl = "no-store";
        ViewData["Title"] = "Chi tiết đơn hàng";
        ViewData["AdminSection"] = "orders";
        ViewData["ShowAdminShell"] = true;
        var model = await orderService.GetOrderDetailsAsync(id, cancellationToken);
        return model is null ? NotFound() : View("OrderDetails", model);
    }

    [HttpPost("orders/{id:int}/status")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateOrderStatus(
        int id,
        string status,
        string? search,
        string? selectedStatus,
        bool returnToDetails = false,
        int page = 1,
        CancellationToken cancellationToken = default)
    {
        var result = await orderService.UpdateStatusAsync(id, status, cancellationToken);
        TempData["AdminNotice"] = result switch
        {
            AdminSaveResult.Success => "Đã cập nhật trạng thái đơn hàng.",
            AdminSaveResult.NotFound => "Không tìm thấy đơn hàng.",
            _ => "Trạng thái đơn hàng không hợp lệ."
        };
        return returnToDetails
            ? RedirectToAction(nameof(OrderDetails), new { id })
            : RedirectToAction(nameof(Orders), new { search, status = selectedStatus, page });
    }

    [HttpGet("feedback")]
    public async Task<IActionResult> Feedback(string? search, CancellationToken cancellationToken)
    {
        Response.Headers.CacheControl = "no-store";
        ViewData["Title"] = "Phản hồi độc giả";
        ViewData["AdminSection"] = "feedback";
        ViewData["ShowAdminShell"] = true;
        var model = await managementService.GetFeedbackAsync(search, cancellationToken);
        return View(model);
    }
}
