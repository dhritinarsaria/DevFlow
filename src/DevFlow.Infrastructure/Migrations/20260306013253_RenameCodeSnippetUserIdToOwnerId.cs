using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DevFlow.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenameCodeSnippetUserIdToOwnerId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CodeSnippets_Users_UserId",
                table: "CodeSnippets");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "CodeSnippets",
                newName: "OwnerId");

            migrationBuilder.RenameIndex(
                name: "IX_CodeSnippets_UserId",
                table: "CodeSnippets",
                newName: "IX_CodeSnippets_OwnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_CodeSnippets_Users_OwnerId",
                table: "CodeSnippets",
                column: "OwnerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CodeSnippets_Users_OwnerId",
                table: "CodeSnippets");

            migrationBuilder.RenameColumn(
                name: "OwnerId",
                table: "CodeSnippets",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_CodeSnippets_OwnerId",
                table: "CodeSnippets",
                newName: "IX_CodeSnippets_UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_CodeSnippets_Users_UserId",
                table: "CodeSnippets",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
