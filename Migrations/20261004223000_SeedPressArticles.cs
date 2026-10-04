using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Nha_Gia_Kim.Data;

#nullable disable

namespace Nha_Gia_Kim.Migrations;

[DbContext(typeof(BookstoreDbContext))]
[Migration("20261004223000_SeedPressArticles")]
public sealed class SeedPressArticles : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.Sql(
            """
            IF NOT EXISTS (
                SELECT 1
                FROM [PressArticles]
                WHERE [BookId] = 1
                  AND [ArticleUrl] = N'https://www.nytimes.com/1993/06/26/books/books-of-the-times-alchemist-comes-to-america.html'
            )
            BEGIN
                INSERT INTO [PressArticles] ([BookId], [PressName], [ArticleTitle], [ArticleUrl], [Snippet])
                VALUES (
                    1,
                    N'The New York Times',
                    N'Books of The Times; Alchemist Comes to America',
                    N'https://www.nytimes.com/1993/06/26/books/books-of-the-times-alchemist-comes-to-america.html',
                    N'Bài điểm sách giới thiệu câu chuyện của Santiago và hành trình theo đuổi kho báu cùng ước mơ.'
                );
            END;

            IF NOT EXISTS (
                SELECT 1
                FROM [PressArticles]
                WHERE [BookId] = 1
                  AND [ArticleUrl] = N'https://www.theguardian.com/childrens-books-site/2015/oct/10/the-alchemist-paulo-coehlo-review'
            )
            BEGIN
                INSERT INTO [PressArticles] ([BookId], [PressName], [ArticleTitle], [ArticleUrl], [Snippet])
                VALUES (
                    1,
                    N'The Guardian',
                    N'The Alchemist by Paulo Coehlo – review',
                    N'https://www.theguardian.com/childrens-books-site/2015/oct/10/the-alchemist-paulo-coehlo-review',
                    N'Bài điểm sách mô tả hành trình của Santiago qua những thử thách, tình yêu và việc khám phá mục đích sống.'
                );
            END;
            """);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
    }
}
