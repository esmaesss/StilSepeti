using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StilSepetiApp.Migrations
{
    /// <inheritdoc />
    public partial class AddProductSeedSupport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Reason",
                table: "ReturnRequests",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "ReviewedAt",
                table: "ReturnRequests",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "userId",
                table: "ReturnRequests",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Orders",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_ReturnRequests_userId",
                table: "ReturnRequests",
                column: "userId");

            migrationBuilder.AddForeignKey(
                name: "FK_ReturnRequests_Users_userId",
                table: "ReturnRequests",
                column: "userId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReturnRequests_Users_userId",
                table: "ReturnRequests");

            migrationBuilder.DropIndex(
                name: "IX_ReturnRequests_userId",
                table: "ReturnRequests");

            migrationBuilder.DropColumn(
                name: "Reason",
                table: "ReturnRequests");

            migrationBuilder.DropColumn(
                name: "ReviewedAt",
                table: "ReturnRequests");

            migrationBuilder.DropColumn(
                name: "userId",
                table: "ReturnRequests");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Orders");
        }
    }
}
