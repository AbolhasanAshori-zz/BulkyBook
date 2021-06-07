using Microsoft.EntityFrameworkCore.Migrations;

namespace BulkyBook.DataAccess.Migrations
{
    public partial class productCorrection : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_CoverTypes_CategoryId",
                table: "Products");

            migrationBuilder.AddColumn<double>(
                name: "ListPrice",
                table: "Products",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.CreateIndex(
                name: "IX_Products_CoverTypeId",
                table: "Products",
                column: "CoverTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_CoverTypes_CoverTypeId",
                table: "Products",
                column: "CoverTypeId",
                principalTable: "CoverTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_CoverTypes_CoverTypeId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_CoverTypeId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "ListPrice",
                table: "Products");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_CoverTypes_CategoryId",
                table: "Products",
                column: "CategoryId",
                principalTable: "CoverTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
