using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Nha_Gia_Kim.Data;

public sealed class BookstoreDbContextFactory : IDesignTimeDbContextFactory<BookstoreDbContext>
{
    public BookstoreDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false)
            .AddEnvironmentVariables()
            .Build();
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("ConnectionStrings:DefaultConnection is not configured.");

        var options = new DbContextOptionsBuilder<BookstoreDbContext>()
            .UseSqlServer(connectionString)
            .Options;

        return new BookstoreDbContext(options);
    }
}
