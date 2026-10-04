using System.Globalization;
using System.Net;
using System.Net.Mail;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Text;
using Nha_Gia_Kim.Models.Api;

namespace Nha_Gia_Kim.Services;

public interface IOrderConfirmationEmailSender
{
    Task<EmailDeliveryResult> SendConfirmationAsync(
        CreateOrderRequest request,
        OrderResponse order,
        CancellationToken cancellationToken = default);
}

public sealed record EmailDeliveryResult(bool Sent, string? FailureReason);

public sealed class OrderConfirmationEmailSender(
    IConfiguration configuration,
    ILogger<OrderConfirmationEmailSender> logger) : IOrderConfirmationEmailSender
{
    public async Task<EmailDeliveryResult> SendConfirmationAsync(
        CreateOrderRequest request,
        OrderResponse order,
        CancellationToken cancellationToken = default)
    {
        var host = configuration["Email:Smtp:Host"];
        var fromAddress = configuration["Email:Smtp:FromAddress"];
        if (string.IsNullOrWhiteSpace(host) || string.IsNullOrWhiteSpace(fromAddress))
        {
            logger.LogWarning("Order {OrderNumber} confirmation email skipped because SMTP host or sender address is not configured.", order.OrderNumber);
            return new EmailDeliveryResult(false, "Email SMTP chưa được cấu hình.");
        }

        var username = configuration["Email:Smtp:Username"];
        var password = configuration["Email:Smtp:Password"];
        if (string.IsNullOrWhiteSpace(username) != string.IsNullOrWhiteSpace(password))
        {
            logger.LogWarning("Order {OrderNumber} confirmation email skipped because SMTP credentials are incomplete.", order.OrderNumber);
            return new EmailDeliveryResult(false, "Cấu hình tài khoản SMTP chưa đầy đủ.");
        }

        try
        {
            var port = configuration.GetValue("Email:Smtp:Port", 587);
            var enableSsl = configuration.GetValue("Email:Smtp:EnableSsl", true);
            var fromName = configuration["Email:Smtp:FromName"] ?? "Nhà Giả Kim";
            using var message = new MailMessage
            {
                From = new MailAddress(fromAddress, fromName, Encoding.UTF8),
                Subject = $"Xác nhận đơn hàng #{order.OrderNumber.ToString("N")[..8].ToUpperInvariant()}",
                SubjectEncoding = Encoding.UTF8,
                BodyEncoding = Encoding.UTF8,
                Body = BuildMessage(request, order),
                IsBodyHtml = false
            };
            message.To.Add(new MailAddress(request.CustomerEmail));

            using var client = new SmtpClient(host, port)
            {
                EnableSsl = enableSsl,
                UseDefaultCredentials = false,
                Timeout = 15000
            };
            if (!string.IsNullOrWhiteSpace(username))
            {
                client.Credentials = new NetworkCredential(username, password);
            }

            using var timeout = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            timeout.CancelAfter(TimeSpan.FromSeconds(20));
            await client.SendMailAsync(message, timeout.Token);
            return new EmailDeliveryResult(true, null);
        }
        catch (SmtpException exception)
        {
            logger.LogError(exception, "SMTP rejected confirmation email for order {OrderNumber}.", order.OrderNumber);
            return new EmailDeliveryResult(false, "Máy chủ email từ chối gửi thư.");
        }
        catch (InvalidOperationException exception)
        {
            logger.LogError(exception, "SMTP is not ready to send confirmation email for order {OrderNumber}.", order.OrderNumber);
            return new EmailDeliveryResult(false, "Máy chủ email chưa sẵn sàng.");
        }
        catch (FormatException exception)
        {
            logger.LogError(exception, "Email address configuration is invalid for order {OrderNumber}.", order.OrderNumber);
            return new EmailDeliveryResult(false, "Địa chỉ email cấu hình không hợp lệ.");
        }
        catch (ArgumentException exception)
        {
            logger.LogError(exception, "SMTP configuration is invalid for order {OrderNumber}.", order.OrderNumber);
            return new EmailDeliveryResult(false, "Cấu hình máy chủ email không hợp lệ.");
        }
        catch (SocketException exception)
        {
            logger.LogError(exception, "SMTP server could not be reached for order {OrderNumber}.", order.OrderNumber);
            return new EmailDeliveryResult(false, "Không kết nối được máy chủ email.");
        }
        catch (AuthenticationException exception)
        {
            logger.LogError(exception, "SMTP TLS negotiation failed for order {OrderNumber}.", order.OrderNumber);
            return new EmailDeliveryResult(false, "Không xác thực được kết nối máy chủ email.");
        }
        catch (IOException exception)
        {
            logger.LogError(exception, "SMTP connection failed for order {OrderNumber}.", order.OrderNumber);
            return new EmailDeliveryResult(false, "Kết nối máy chủ email bị gián đoạn.");
        }
        catch (OperationCanceledException exception)
        {
            logger.LogError(exception, "Sending confirmation email timed out for order {OrderNumber}.", order.OrderNumber);
            return new EmailDeliveryResult(false, "Gửi email bị quá thời gian chờ.");
        }
    }

    private static string BuildMessage(CreateOrderRequest request, OrderResponse order)
    {
        var culture = CultureInfo.GetCultureInfo("vi-VN");
        var lines = order.Items.Select(item =>
            $"- {item.BookTitle}: {item.Quantity} cuốn × {item.UnitPrice.ToString("N0", culture)} đ = {item.LineTotal.ToString("N0", culture)} đ");

        return string.Join(
            Environment.NewLine,
            [
                $"Xin chào {request.CustomerName},",
                "",
                "Nhà Giả Kim đã ghi nhận đơn hàng của bạn.",
                $"Mã đơn hàng: {order.OrderNumber}",
                $"Ngày đặt: {order.CreatedAt.ToLocalTime():dd/MM/yyyy HH:mm}",
                "",
                "Thông tin khách hàng:",
                $"Họ tên: {request.CustomerName}",
                $"Email: {request.CustomerEmail}",
                $"Số điện thoại: {request.CustomerPhone}",
                $"Địa chỉ giao hàng: {request.ShippingAddress}, {request.ProvinceCity}",
                $"Phương thức thanh toán: {GetPaymentMethodName(request.PaymentMethod)}",
                "",
                "Sách đã đặt:",
                .. lines,
                "",
                $"Tổng số lượng: {order.Items.Sum(item => item.Quantity)} cuốn",
                $"Tổng thanh toán: {order.TotalAmount.ToString("N0", culture)} đ",
                "",
                "Cảm ơn bạn đã mua sách tại Nhà Giả Kim."
            ]);
    }

    private static string GetPaymentMethodName(string paymentMethod) => paymentMethod switch
    {
        "CashOnDelivery" => "COD · Thanh toán khi nhận hàng",
        "BankTransfer" => "Chuyển khoản · thanh toán theo hướng dẫn sau",
        "DigitalWallet" => "Ví điện tử · thanh toán theo hướng dẫn sau",
        _ => paymentMethod
    };
}
