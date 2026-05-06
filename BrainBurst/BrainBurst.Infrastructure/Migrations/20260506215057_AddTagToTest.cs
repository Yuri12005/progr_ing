using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BrainBurst.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTagToTest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TagId",
                table: "Tests",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tests_TagId",
                table: "Tests",
                column: "TagId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tests_Tags_TagId",
                table: "Tests",
                column: "TagId",
                principalTable: "Tags",
                principalColumn: "TagId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tests_Tags_TagId",
                table: "Tests");

            migrationBuilder.DropIndex(
                name: "IX_Tests_TagId",
                table: "Tests");

            migrationBuilder.DropColumn(
                name: "TagId",
                table: "Tests");
        }
    }
}
