using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nha_Gia_Kim.Migrations
{
    /// <inheritdoc />
    public partial class AddCheckoutDeliveryDetailsAndRelatedCovers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PaymentMethod",
                table: "Orders",
                type: "nvarchar(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "CashOnDelivery");

            migrationBuilder.AddColumn<string>(
                name: "ProvinceCity",
                table: "Orders",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "Khác");

            migrationBuilder.UpdateData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 2,
                column: "CoverImageUrl",
                value: "/images/dac-nhan-tam.png");

            migrationBuilder.UpdateData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 3,
                column: "CoverImageUrl",
                value: "/images/tuoi-tre-dang-gia-bao-nhieu.png");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaymentMethod",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "ProvinceCity",
                table: "Orders");

            migrationBuilder.UpdateData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 2,
                column: "CoverImageUrl",
                value: null);

            migrationBuilder.UpdateData(
                table: "Books",
                keyColumn: "Id",
                keyValue: 3,
                column: "CoverImageUrl",
                value: null);
        }
    }
}
