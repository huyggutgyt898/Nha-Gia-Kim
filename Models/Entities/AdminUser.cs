namespace Nha_Gia_Kim.Models.Entities;

public sealed class AdminUser
{
    public int Id { get; set; }
    public required string Username { get; set; }
    public required string PasswordHash { get; set; }
    public string? FullName { get; set; }
    public string Role { get; set; } = "Admin";
}
