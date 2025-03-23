using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PersonalFinanceDB2.Migrations
{
    /// <inheritdoc />
    public partial class AddReceiptDiscountToReceipts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "ReceiptDiscount",
                table: "Receipts",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AlterColumn<int>(
                name: "SubcategoryID",
                table: "Products",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReceiptDiscount",
                table: "Receipts");

            migrationBuilder.AlterColumn<int>(
                name: "SubcategoryID",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}
