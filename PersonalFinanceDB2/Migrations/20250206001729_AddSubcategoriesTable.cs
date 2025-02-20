using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PersonalFinanceDB2.Migrations
{
    /// <inheritdoc />
    public partial class AddSubcategoriesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Receipts_Stores_StoreID",
                table: "Receipts");

            migrationBuilder.CreateTable(
                name: "Subcategories",
                columns: table => new
                {
                    SubcategoryID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CategoryID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subcategories", x => x.SubcategoryID);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_Receipts_Stores_StoreID",
                table: "Receipts",
                column: "StoreID",
                principalTable: "Stores",
                principalColumn: "StoreID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Receipts_Stores_StoreID",
                table: "Receipts");

            migrationBuilder.DropTable(
                name: "Subcategories");

            migrationBuilder.AddForeignKey(
                name: "FK_Receipts_Stores_StoreID",
                table: "Receipts",
                column: "StoreID",
                principalTable: "Stores",
                principalColumn: "StoreID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
