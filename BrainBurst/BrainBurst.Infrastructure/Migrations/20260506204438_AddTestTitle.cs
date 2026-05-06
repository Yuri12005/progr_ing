using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BrainBurst.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTestTitle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "Tests",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Title",
                table: "Tests");
        }
    }
}
