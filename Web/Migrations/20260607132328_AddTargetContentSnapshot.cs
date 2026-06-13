using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StudyShare.Migrations
{
    /// <inheritdoc />
    public partial class AddTargetContentSnapshot : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TargetContentSnapshot",
                table: "Reports",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TargetContentSnapshot",
                table: "Reports");
        }
    }
}
