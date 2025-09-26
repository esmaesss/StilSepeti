using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StilSepetiApp.Migrations
{
    /// <inheritdoc />
    public partial class FixCascadeDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReturnRequests_Users_UserId",
                table: "ReturnRequests");

            migrationBuilder.AddForeignKey(
    name: "FK_ReturnRequests_Users_userId",
    table: "ReturnRequests",
    column: "userId",
    principalTable: "Users",
    principalColumn: "Id",
    onDelete: ReferentialAction.NoAction); 
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReturnRequests_Users_UserId",
                table: "ReturnRequests");

            migrationBuilder.AddForeignKey(
                name: "FK_ReturnRequests_Users_UserId",
                table: "ReturnRequests",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "userId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
