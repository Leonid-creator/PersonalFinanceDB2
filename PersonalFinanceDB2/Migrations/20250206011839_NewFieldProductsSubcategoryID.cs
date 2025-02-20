using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PersonalFinanceDB2.Migrations
{
    /// <inheritdoc />
    public partial class NewFieldProductsSubcategoryID : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SubcategoryID",
                table: "Products",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SubcategoryID",
                table: "Products");
        }
    }
}
