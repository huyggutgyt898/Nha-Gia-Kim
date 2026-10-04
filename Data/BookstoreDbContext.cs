using Microsoft.EntityFrameworkCore;
using Nha_Gia_Kim.Models.Entities;

namespace Nha_Gia_Kim.Data;

public sealed class BookstoreDbContext(DbContextOptions<BookstoreDbContext> options) : DbContext(options)
{
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Book> Books => Set<Book>();
    public DbSet<Author> Authors => Set<Author>();
    public DbSet<PressArticle> PressArticles => Set<PressArticle>();
    public DbSet<BookReview> Feedbacks => Set<BookReview>();
    public DbSet<AdminUser> Users => Set<AdminUser>();
    public DbSet<Cart> Carts => Set<Cart>();
    public DbSet<CartItem> CartItems => Set<CartItem>();
    public DbSet<CustomerOrder> Orders => Set<CustomerOrder>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>(entity =>
        {
            entity.HasKey(category => category.Id);
            entity.Property(category => category.Name).HasMaxLength(255).IsRequired();
            entity.Property(category => category.Slug).HasMaxLength(120).IsRequired();
            entity.HasIndex(category => category.Name).IsUnique();
            entity.HasIndex(category => category.Slug).IsUnique();
            entity.HasData(
                new Category { Id = 1, Name = "Văn học", Slug = "van-hoc" },
                new Category { Id = 2, Name = "Kinh doanh", Slug = "kinh-doanh" },
                new Category { Id = 3, Name = "Kỹ năng sống", Slug = "ky-nang-song" });
        });

        modelBuilder.Entity<Book>(entity =>
        {
            entity.HasKey(book => book.Id);
            entity.Property(book => book.Title).HasMaxLength(255).IsRequired();
            entity.Property(book => book.Subtitle).HasMaxLength(500);
            entity.Property(book => book.Isbn).HasMaxLength(20);
            entity.Property(book => book.Description).HasColumnType("nvarchar(max)");
            entity.Property(book => book.CoverImageUrl).HasMaxLength(500);
            entity.Property(book => book.FilePreviewUrl).HasMaxLength(500);
            entity.Property(book => book.Price).HasPrecision(10, 2);
            entity.Property(book => book.OriginalPrice).HasPrecision(10, 2);
            entity.Property(book => book.RowVersion).IsRowVersion();
            entity.HasIndex(book => book.CategoryId);
            entity.HasIndex(book => book.AuthorId);
            entity.ToTable(table =>
            {
                table.HasCheckConstraint("CK_Books_Price", "[Price] >= 0");
                table.HasCheckConstraint("CK_Books_StockQuantity", "[StockQuantity] >= 0");
            });
            entity.HasOne(book => book.Author)
                .WithMany(author => author.Books)
                .HasForeignKey(book => book.AuthorId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(book => book.Category)
                .WithMany(category => category.Books)
                .HasForeignKey(book => book.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasData(
                new Book
                {
                    Id = 1, Title = "Nhà Giả Kim", AuthorId = 1, Isbn = "9786043121197",
                    Description = "Hành trình theo đuổi ước mơ và lắng nghe tiếng gọi của trái tim.",
                    CoverImageUrl = "/images/nha-gia-kim-bia.png",
                    Price = 79000, StockQuantity = 30, CategoryId = 1, IsActive = true,
                    CreatedAt = new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero)
                },
                new Book
                {
                    Id = 2, Title = "Đắc Nhân Tâm", AuthorId = 2, Isbn = "9786041",
                    Description = "Những nguyên tắc thiết thực để thấu hiểu và kết nối với mọi người.",
                    CoverImageUrl = "/images/dac-nhan-tam.png",
                    Price = 86000, StockQuantity = 25, CategoryId = 3, IsActive = true,
                    CreatedAt = new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero)
                },
                new Book
                {
                    Id = 3, Title = "Tuổi Trẻ Đáng Giá Bao Nhiêu", AuthorId = 3,
                    Description = "Gợi ý để học hỏi, trải nghiệm và sống trọn vẹn hơn.",
                    CoverImageUrl = "/images/tuoi-tre-dang-gia-bao-nhieu.png",
                    Price = 72000, StockQuantity = 18, CategoryId = 3, IsActive = true,
                    CreatedAt = new DateTimeOffset(2025, 1, 1, 0, 0, 0, TimeSpan.Zero)
                });
        });

        modelBuilder.Entity<Author>(entity =>
        {
                entity.HasKey(author => author.Id);
                entity.Property(author => author.Name).HasMaxLength(255).IsRequired();
                entity.Property(author => author.Bio).HasColumnType("nvarchar(max)");
                entity.Property(author => author.AvatarUrl).HasMaxLength(500);
                entity.HasData(
                    new Author { Id = 1, Name = "Paulo Coelho" },
                    new Author { Id = 2, Name = "Dale Carnegie" },
                    new Author { Id = 3, Name = "Rosie Nguyễn" });
        });

        modelBuilder.Entity<PressArticle>(entity =>
        {
                entity.HasKey(article => article.Id);
                entity.Property(article => article.PressName).HasMaxLength(255).IsRequired();
                entity.Property(article => article.ArticleTitle).HasMaxLength(500).IsRequired();
                entity.Property(article => article.ArticleUrl).HasMaxLength(500);
                entity.Property(article => article.Snippet).HasColumnType("nvarchar(max)");
                entity.HasIndex(article => article.BookId);
                entity.HasOne(article => article.Book)
                    .WithMany()
                    .HasForeignKey(article => article.BookId)
                    .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<BookReview>(entity =>
        {
                entity.HasKey(review => review.Id);
                entity.ToTable("Feedbacks", table =>
                    table.HasCheckConstraint("CK_Feedbacks_Rating", "[Rating] >= 1 AND [Rating] <= 5"));
                entity.Property(review => review.Id).HasColumnName("FeedbackId");
                entity.Property(review => review.DisplayName).HasColumnName("CustomerName").HasMaxLength(255).IsRequired();
                entity.Property(review => review.Content).HasColumnName("Comment").HasColumnType("nvarchar(max)");
                entity.HasIndex(review => new { review.BookId, review.CreatedAt });
                entity.HasOne(review => review.Book)
                    .WithMany(book => book.Reviews)
                    .HasForeignKey(review => review.BookId)
                    .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<AdminUser>(entity =>
        {
                entity.ToTable("Users");
                entity.HasKey(user => user.Id);
                entity.Property(user => user.Username).HasMaxLength(100).IsRequired();
                entity.Property(user => user.PasswordHash).HasMaxLength(255).IsRequired();
                entity.Property(user => user.FullName).HasMaxLength(255);
                entity.Property(user => user.Role).HasMaxLength(50).HasDefaultValue("Admin").IsRequired();
                entity.HasIndex(user => user.Username).IsUnique();
        });

        modelBuilder.Entity<Cart>(entity =>
        {
            entity.HasKey(cart => cart.Id);
            entity.HasMany(cart => cart.Items)
                .WithOne(item => item.Cart)
                .HasForeignKey(item => item.CartId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<CartItem>(entity =>
        {
            entity.HasKey(item => new { item.CartId, item.BookId });
            entity.HasOne(item => item.Book)
                .WithMany()
                .HasForeignKey(item => item.BookId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.ToTable(table => table.HasCheckConstraint("CK_CartItems_Quantity", "[Quantity] > 0"));
        });

        modelBuilder.Entity<CustomerOrder>(entity =>
        {
            entity.HasKey(order => order.Id);
            entity.HasIndex(order => order.OrderNumber).IsUnique();
            entity.Property(order => order.CustomerName).HasMaxLength(150).IsRequired();
            entity.Property(order => order.CustomerEmail).HasMaxLength(254).IsRequired();
            entity.Property(order => order.CustomerPhone).HasMaxLength(30).IsRequired();
            entity.Property(order => order.ShippingAddress).HasMaxLength(500).IsRequired();
            entity.Property(order => order.ProvinceCity).HasColumnName("Province").HasMaxLength(100).IsRequired();
            entity.Property(order => order.PaymentMethod).HasMaxLength(50).IsRequired();
            entity.Property(order => order.Status).HasMaxLength(50).IsRequired();
            entity.Property(order => order.TotalAmount).HasPrecision(10, 2);
            entity.ToTable(table =>
                table.HasCheckConstraint(
                    "CK_Orders_Status",
                    "[Status] IN (N'Đăng ký', N'Đã gửi', N'Đang giao', N'Đã giao')"));
            entity.HasMany(order => order.Items)
                .WithOne(item => item.Order)
                .HasForeignKey(item => item.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(item => item.Id);
            entity.Property(item => item.BookTitle).HasMaxLength(255).IsRequired();
            entity.Property(item => item.UnitPrice).HasPrecision(10, 2);
            entity.HasOne<Book>()
                .WithMany()
                .HasForeignKey(item => item.BookId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.ToTable(table => table.HasCheckConstraint("CK_OrderItems_Quantity", "[Quantity] > 0"));
        });
    }
}
