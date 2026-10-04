using System.Security.Claims;

namespace Nha_Gia_Kim.Services;

public interface IAdminAuthenticationService
{
    Task<ClaimsPrincipal?> AuthenticateAsync(
        string username,
        string password,
        CancellationToken cancellationToken);
}
