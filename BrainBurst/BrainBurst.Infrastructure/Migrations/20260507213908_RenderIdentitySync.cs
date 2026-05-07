using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace BrainBurst.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenderIdentitySync : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Flashcards_User_UserId",
                table: "Flashcards");

            migrationBuilder.DropForeignKey(
                name: "FK_Tags_User_CreatorId",
                table: "Tags");

            migrationBuilder.DropForeignKey(
                name: "FK_Tags_Users_ApplicationUserId",
                table: "Tags");

            migrationBuilder.DropForeignKey(
                name: "FK_TestResults_User_UserId",
                table: "TestResults");

            migrationBuilder.DropForeignKey(
                name: "FK_TestResults_Users_ApplicationUserId",
                table: "TestResults");

            migrationBuilder.DropForeignKey(
                name: "FK_Tests_User_CreatorId",
                table: "Tests");

            migrationBuilder.DropForeignKey(
                name: "FK_Tests_Users_ApplicationUserId",
                table: "Tests");

            migrationBuilder.DropTable(
                name: "User");

            migrationBuilder.DropIndex(
                name: "IX_Tests_ApplicationUserId",
                table: "Tests");

            migrationBuilder.DropIndex(
                name: "IX_TestResults_ApplicationUserId",
                table: "TestResults");

            migrationBuilder.DropIndex(
                name: "IX_Tags_ApplicationUserId",
                table: "Tags");

            migrationBuilder.DropIndex(
                name: "IX_Flashcards_UserId",
                table: "Flashcards");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "Tests");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "TestResults");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Flashcards");

            migrationBuilder.AddForeignKey(
                name: "FK_Tags_Users_CreatorId",
                table: "Tags",
                column: "CreatorId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_TestResults_Users_UserId",
                table: "TestResults",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Tests_Users_CreatorId",
                table: "Tests",
                column: "CreatorId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tags_Users_CreatorId",
                table: "Tags");

            migrationBuilder.DropForeignKey(
                name: "FK_TestResults_Users_UserId",
                table: "TestResults");

            migrationBuilder.DropForeignKey(
                name: "FK_Tests_Users_CreatorId",
                table: "Tests");

            migrationBuilder.AddColumn<int>(
                name: "ApplicationUserId",
                table: "Tests",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ApplicationUserId",
                table: "TestResults",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ApplicationUserId",
                table: "Tags",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Flashcards",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    FullName = table.Column<string>(type: "text", nullable: true),
                    PasswordHash = table.Column<string>(type: "text", nullable: false),
                    Points = table.Column<int>(type: "integer", nullable: false),
                    Rank = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.UserId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tests_ApplicationUserId",
                table: "Tests",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_TestResults_ApplicationUserId",
                table: "TestResults",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_ApplicationUserId",
                table: "Tags",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Flashcards_UserId",
                table: "Flashcards",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Flashcards_User_UserId",
                table: "Flashcards",
                column: "UserId",
                principalTable: "User",
                principalColumn: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tags_User_CreatorId",
                table: "Tags",
                column: "CreatorId",
                principalTable: "User",
                principalColumn: "UserId",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Tags_Users_ApplicationUserId",
                table: "Tags",
                column: "ApplicationUserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TestResults_User_UserId",
                table: "TestResults",
                column: "UserId",
                principalTable: "User",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TestResults_Users_ApplicationUserId",
                table: "TestResults",
                column: "ApplicationUserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Tests_User_CreatorId",
                table: "Tests",
                column: "CreatorId",
                principalTable: "User",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Tests_Users_ApplicationUserId",
                table: "Tests",
                column: "ApplicationUserId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
