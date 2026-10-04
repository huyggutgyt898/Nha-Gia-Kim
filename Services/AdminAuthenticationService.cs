using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Nha_Gia_Kim.Data;
using Nha_Gia_Kim.Models.Entities;

namespace Nha_Gia_Kim.Services;

public sealed class AdminAuthenticationService(
    BookstoreDbContext dbContext,
    IPasswordHasher<AdminUser> passwordHasher) : IAdminAuthenticationService
{
    public async Task<ClaimsPrincipal?> AuthenticateAsync(
        string username,
        string password,
        CancellationToken cancellationToken)
    {
        var normalizedUsername = username.Trim().ToUpperInvariant();
        var admin = await dbContext.Users.SingleOrDefaultAsync(
            user => user.Username == normalizedUsername,
            cancellationToken);

        if (admin is null || !string.Equals(admin.Role, "Admin", StringComparison.Ordinal))
        {
            return null;
        }

        var verificationResult = passwordHasher.VerifyHashedPassword(admin, admin.PasswordHash, password);
        if (verificationResult == PasswordVerificationResult.Failed)
        {
            return null;
        }

        if (verificationResult == PasswordVerificationResult.SuccessRehashNeeded)
        {
            admin.PasswordHash = passwordHasher.HashPassword(admin, password);
            await dbContext.SaveChangesAsync(cancellationToken);
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, admin.Id.ToString()),
            new(ClaimTypes.Name, string.IsNullOrWhiteSpace(admin.FullName) ? admin.Username : admin.FullName),
            new(ClaimTypes.Role, admin.Role)
        };

        return new ClaimsPrincipal(
            new ClaimsIdentity(claims, "AdminCookie", ClaimTypes.Name, ClaimTypes.Role));
    }
}
