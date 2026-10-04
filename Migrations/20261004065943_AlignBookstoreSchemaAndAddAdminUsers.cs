using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Nha_Gia_Kim.Migrations
{
    /// <inheritdoc />
    public partial class AlignBookstoreSchemaAndAddAdminUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BookReviews_Books_BookId",
                table: "BookReviews");

            migrationBuilder.DropPrimaryKey(
                name: "PK_BookReviews",
                table: "BookReviews");

            migrationBuilder.DropCheckConstraint(
                name: "CK_BookReviews_Rating",
                table: "BookReviews");

            migrationBuilder.RenameTable(
                name: "BookReviews",
                newName: "Feedbacks");

            migrationBuilder.RenameColumn(
                name: "ProvinceCity",
                table: "Orders",
                newName: "Province");

            migrationBuilder.RenameColumn(
                name: "DisplayName",
                table: "Feedbacks",
                newName: "CustomerName");

            migrationBuilder.RenameColumn(
                name: "Content",
                table: "Feedbacks",
                newName: "Comment");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Feedbacks",
                newName: "FeedbackId");

            migrationBuilder.RenameIndex(
                name: "IX_BookReviews_BookId_CreatedAt",
                table: "Feedbacks",
                newName: "IX_Feedbacks_BookId_CreatedAt");

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalAmount",
                table: "Orders",
                type: "decimal(10,2)",
                precision: 10,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2);

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Orders",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(30)",
                oldMaxLength: 30);

            migrationBuilder.AlterColumn<string>(
                name: "PaymentMethod",
                table: "Orders",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(30)",
                oldMaxLength: 30);

            migrationBuilder.AlterColumn<decimal>(
                name: "UnitPrice",
                table: "OrderItems",
                type: "decimal(10,2)",
                precision: 10,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2);

            migrationBuilder.AlterColumn<string>(
                name: "BookTitle",
                table: "OrderItems",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(250)",
                oldMaxLength: 250);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Categories",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Books",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(250)",
                oldMaxLength: 250);

            migrationBuilder.AlterColumn<decimal>(
                name: "Price",
                table: "Books",
                type: "decimal(10,2)",
                precision: 10,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Books",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(4000)",
                oldMaxLength: 4000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CoverImageUrl",
                table: "Books",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AuthorId",
                table: "Books",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FilePreviewUrl",
                table: "Books",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "OriginalPrice",
                table: "Books",
                type: "decimal(10,2)",
                precision: 10,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Subtitle",
                table: "Books",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CustomerName",
                table: "Feedbacks",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(80)",
                oldMaxLength: 80);

            migrationBuilder.AlterColumn<string>(
                name: "Comment",
                table: "Feedbacks",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Feedbacks",
                table: "Feedbacks",
                column: "FeedbackId");

            migrationBuilder.CreateTable(
                name: "Authors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Bio = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AvatarUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Authors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PressArticles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BookId = table.Column<int>(type: "int", nullable: false),
                    PressName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ArticleTitle = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ArticleUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Snippet = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PressArticles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PressArticles_Books_BookId",
                        column: x => x.BookId,
                        principalTable: "Books",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Role = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "Admin")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Authors",
                columns: new[] { "Id", "AvatarUrl", "Bio", "Name" },
                values: new object[,]
                {
                    { 1, null, null, "Paulo Coelho" },
                    { 2, null, null, "Dale Carnegie" },
                    { 3, null, null, "Rosie Nguyễn" }
                });

            migrationBuilder.Sql(
                """
                INSERT INTO [Authors] ([Name])
                SELECT DISTINCT CASE
                    WHEN NULLIF(LTRIM(RTRIM([Books].[Author])), N'') IS NULL THEN N'Chưa rõ tác giả'
                    ELSE LTRIM(RTRIM([Books].[Author]))
                END
                FROM [Books]
                WHERE NOT EXISTS
                (
                    SELECT 1
                    FROM [Authors]
                    WHERE [Authors].[Name] = CASE
                        WHEN NULLIF(LTRIM(RTRIM([Books].[Author])), N'') IS NULL THEN N'Chưa rõ tác giả'
                        ELSE LTRIM(RTRIM([Books].[Author]))
                    END
                );

                UPDATE [Books]
                SET [Books].[AuthorId] = [Authors].[Id]
                FROM [Books]
                INNER JOIN [Authors]
                    ON [Authors].[Name] = CASE
                        WHEN NULLIF(LTRIM(RTRIM([Books].[Author])), N'') IS NULL THEN N'Chưa rõ tác giả'
                        ELSE LTRIM(RTRIM([Books].[Author]))
                    END;
                """);

            migrationBuilder.AlterColumn<int>(
                name: "AuthorId",
                table: "Books",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "FilePreviewUrl", "OriginalPrice", "Subtitle" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "FilePreviewUrl", "OriginalPrice", "Subtitle" },
                values: new object[] { null, null, null });

            migrationBuilder.UpdateData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "FilePreviewUrl", "OriginalPrice", "Subtitle" },
                values: new object[] { null, null, null });

            migrationBuilder.Sql(
                """
                UPDATE [Orders]
                SET [Status] = CASE LTRIM(RTRIM([Status]))
                    WHEN N'Pending' THEN N'Đăng ký'
                    WHEN N'Registered' THEN N'Đăng ký'
                    WHEN N'Sent' THEN N'Đã gửi'
                    WHEN N'Shipped' THEN N'Đã gửi'
                    WHEN N'InTransit' THEN N'Đang giao'
                    WHEN N'Delivered' THEN N'Đã giao'
                    ELSE [Status]
                END;
                """);

            migrationBuilder.AddCheckConstraint(
                name: "CK_Orders_Status",
                table: "Orders",
                sql: "[Status] IN (N'Đăng ký', N'Đã gửi', N'Đang giao', N'Đã giao')");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_Name",
                table: "Categories",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Books_AuthorId",
                table: "Books",
                column: "AuthorId");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Feedbacks_Rating",
                table: "Feedbacks",
                sql: "[Rating] >= 1 AND [Rating] <= 5");

            migrationBuilder.CreateIndex(
                name: "IX_PressArticles_BookId",
                table: "PressArticles",
                column: "BookId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Books_Authors_AuthorId",
                table: "Books",
                column: "AuthorId",
                principalTable: "Authors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Feedbacks_Books_BookId",
                table: "Feedbacks",
                column: "BookId",
                principalTable: "Books",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.DropColumn(
                name: "Author",
                table: "Books");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Books_Authors_AuthorId",
                table: "Books");

            migrationBuilder.DropForeignKey(
                name: "FK_Feedbacks_Books_BookId",
                table: "Feedbacks");

            migrationBuilder.DropTable(
                name: "PressArticles");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Orders_Status",
                table: "Orders");

            migrationBuilder.Sql(
                """
                UPDATE [Orders]
                SET [Status] = CASE [Status]
                    WHEN N'Đăng ký' THEN N'Pending'
                    WHEN N'Đã gửi' THEN N'Sent'
                    WHEN N'Đang giao' THEN N'InTransit'
                    WHEN N'Đã giao' THEN N'Delivered'
                    ELSE [Status]
                END;
                """);

            migrationBuilder.DropIndex(
                name: "IX_Categories_Name",
                table: "Categories");

            migrationBuilder.DropIndex(
                name: "IX_Books_AuthorId",
                table: "Books");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Feedbacks",
                table: "Feedbacks");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Feedbacks_Rating",
                table: "Feedbacks");

            migrationBuilder.DropColumn(
                name: "FilePreviewUrl",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "OriginalPrice",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "Subtitle",
                table: "Books");

            migrationBuilder.AddColumn<string>(
                name: "Author",
                table: "Books",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.Sql(
                """
                UPDATE [Books]
                SET [Books].[Author] = [Authors].[Name]
                FROM [Books]
                INNER JOIN [Authors] ON [Authors].[Id] = [Books].[AuthorId];
                """);

            migrationBuilder.DropColumn(
                name: "AuthorId",
                table: "Books");

            migrationBuilder.DropTable(
                name: "Authors");

            migrationBuilder.AlterColumn<string>(
                name: "Author",
                table: "Books",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.RenameTable(
                name: "Feedbacks",
                newName: "BookReviews");

            migrationBuilder.RenameColumn(
                name: "Province",
                table: "Orders",
                newName: "ProvinceCity");

            migrationBuilder.RenameColumn(
                name: "CustomerName",
                table: "BookReviews",
                newName: "DisplayName");

            migrationBuilder.RenameColumn(
                name: "Comment",
                table: "BookReviews",
                newName: "Content");

            migrationBuilder.RenameColumn(
                name: "FeedbackId",
                table: "BookReviews",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "IX_Feedbacks_BookId_CreatedAt",
                table: "BookReviews",
                newName: "IX_BookReviews_BookId_CreatedAt");

            migrationBuilder.AlterColumn<decimal>(
                name: "TotalAmount",
                table: "Orders",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)",
                oldPrecision: 10,
                oldScale: 2);

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Orders",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "PaymentMethod",
                table: "Orders",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<decimal>(
                name: "UnitPrice",
                table: "OrderItems",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)",
                oldPrecision: 10,
                oldScale: 2);

            migrationBuilder.AlterColumn<string>(
                name: "BookTitle",
                table: "OrderItems",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Categories",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Books",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<decimal>(
                name: "Price",
                table: "Books",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,2)",
                oldPrecision: 10,
                oldScale: 2);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Books",
                type: "nvarchar(4000)",
                maxLength: 4000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CoverImageUrl",
                table: "Books",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DisplayName",
                table: "BookReviews",
                type: "nvarchar(80)",
                maxLength: 80,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<string>(
                name: "Content",
                table: "BookReviews",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_BookReviews",
                table: "BookReviews",
                column: "Id");

            migrationBuilder.AddCheckConstraint(
                name: "CK_BookReviews_Rating",
                table: "BookReviews",
                sql: "[Rating] >= 1 AND [Rating] <= 5");

            migrationBuilder.AddForeignKey(
                name: "FK_BookReviews_Books_BookId",
                table: "BookReviews",
                column: "BookId",
                principalTable: "Books",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
