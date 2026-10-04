using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nha_Gia_Kim.Migrations
{
    /// <inheritdoc />
    public partial class AddBookIntegrityConstraints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddCheckConstraint(
                name: "CK_Books_Price",
                table: "Books",
                sql: "[Price] >= 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Books_StockQuantity",
                table: "Books",
                sql: "[StockQuantity] >= 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Books_Price",
                table: "Books");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Books_StockQuantity",
                table: "Books");
        }
    }
}
