namespace Nha_Gia_Kim.Models.Entities;

public static class OrderStatuses
{
    public const string Registered = "Đăng ký";
    public const string Sent = "Đã gửi";
    public const string InTransit = "Đang giao";
    public const string Delivered = "Đã giao";

    public static IReadOnlyList<string> All { get; } =
    [
        Registered,
        Sent,
        InTransit,
        Delivered
    ];

    public static bool Contains(string? status) =>
        status is Registered or Sent or InTransit or Delivered;
}
