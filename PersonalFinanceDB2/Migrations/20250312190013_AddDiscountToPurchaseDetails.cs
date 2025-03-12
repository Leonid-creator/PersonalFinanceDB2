using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PersonalFinanceDB2.Migrations
{
    /// <inheritdoc />
    public partial class AddDiscountToPurchaseDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Discount",
                table: "PurchaseDetails",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Discount",
                table: "PurchaseDetails");
        }
    }
}
