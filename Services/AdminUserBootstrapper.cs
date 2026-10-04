using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Nha_Gia_Kim.Data;
using Nha_Gia_Kim.Models.Entities;

namespace Nha_Gia_Kim.Services;

public sealed class AdminUserBootstrapper(
    BookstoreDbContext dbContext,
    IPasswordHasher<AdminUser> passwordHasher,
    IConfiguration configuration,
    ILogger<AdminUserBootstrapper> logger) : IAdminUserBootstrapper
{
    public async Task EnsureInitialAdminAsync(CancellationToken cancellationToken)
    {
        if (await dbContext.Users.AnyAsync(cancellationToken))
        {
            return;
        }

        var usernameValue = configuration["Admin:InitialUsername"];
        var password = configuration["Admin:InitialPassword"];
        if (string.IsNullOrWhiteSpace(usernameValue) && string.IsNullOrWhiteSpace(password))
        {
            logger.LogWarning(
                "No admin account exists. Configure Admin:InitialUsername and Admin:InitialPassword to bootstrap one.");
            return;
        }

        if (string.IsNullOrWhiteSpace(usernameValue) || string.IsNullOrWhiteSpace(password))
        {
            throw new InvalidOperationException(
                "Configure both Admin:InitialUsername and Admin:InitialPassword to bootstrap the first admin account.");
        }

        var username = usernameValue.Trim().ToUpperInvariant();
        var fullName = configuration["Admin:InitialFullName"]?.Trim();
        if (username.Length > 100 || password.Length < 12 || fullName?.Length > 255)
        {
            throw new InvalidOperationException(
                "The initial admin username must be at most 100 characters, the password must be at least 12 characters, and the full name must be at most 255 characters.");
        }

        var admin = new AdminUser
        {
            Username = username,
            FullName = fullName,
            Role = "Admin",
            PasswordHash = string.Empty
        };
        admin.PasswordHash = passwordHasher.HashPassword(admin, password);

        dbContext.Users.Add(admin);
        await dbContext.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Bootstrapped the initial admin account.");
    }
}
