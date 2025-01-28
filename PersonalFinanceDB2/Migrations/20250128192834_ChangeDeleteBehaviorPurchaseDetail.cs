using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PersonalFinanceDB2.Migrations
{
    /// <inheritdoc />
    public partial class ChangeDeleteBehaviorPurchaseDetail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseDetails_Products_ProductID",
                table: "PurchaseDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseDetails_Receipts_ReceiptID",
                table: "PurchaseDetails");

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseDetails_Products_ProductID",
                table: "PurchaseDetails",
                column: "ProductID",
                principalTable: "Products",
                principalColumn: "ProductID");

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseDetails_Receipts_ReceiptID",
                table: "PurchaseDetails",
                column: "ReceiptID",
                principalTable: "Receipts",
                principalColumn: "ReceiptID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseDetails_Products_ProductID",
                table: "PurchaseDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseDetails_Receipts_ReceiptID",
                table: "PurchaseDetails");

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseDetails_Products_ProductID",
                table: "PurchaseDetails",
                column: "ProductID",
                principalTable: "Products",
                principalColumn: "ProductID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseDetails_Receipts_ReceiptID",
                table: "PurchaseDetails",
                column: "ReceiptID",
                principalTable: "Receipts",
                principalColumn: "ReceiptID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
