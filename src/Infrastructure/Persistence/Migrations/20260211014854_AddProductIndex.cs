using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddProductIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "idx_products_active_category_price",
                table: "Products",
                columns: new[] { "Category", "Price" },
                filter: "\"Active\" = true");

            migrationBuilder.CreateIndex(
                name: "idx_products_active_price",
                table: "Products",
                column: "Price",
                filter: "\"Active\" = true");

            migrationBuilder.CreateIndex(
                name: "idx_products_category",
                table: "Products",
                column: "Category");

            migrationBuilder.CreateIndex(
                name: "idx_products_status",
                table: "Products",
                column: "Active");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "idx_products_active_category_price",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "idx_products_active_price",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "idx_products_category",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "idx_products_status",
                table: "Products");
        }
    }
}
