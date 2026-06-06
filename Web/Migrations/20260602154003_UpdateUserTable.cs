using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudyShare.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUserTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reports_Documents_DocumentId",
                table: "Reports");

            migrationBuilder.AddForeignKey(
                name: "FK_Reports_Documents_DocumentId",
                table: "Reports",
                column: "DocumentId",
                principalTable: "Documents",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reports_Documents_DocumentId",
                table: "Reports");

            migrationBuilder.AddForeignKey(
                name: "FK_Reports_Documents_DocumentId",
                table: "Reports",
                column: "DocumentId",
                principalTable: "Documents",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
