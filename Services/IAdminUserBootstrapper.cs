namespace Nha_Gia_Kim.Services;

public interface IAdminUserBootstrapper
{
    Task EnsureInitialAdminAsync(CancellationToken cancellationToken);
}
