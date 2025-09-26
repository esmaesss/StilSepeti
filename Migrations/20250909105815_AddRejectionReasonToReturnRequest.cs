using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace StilSepetiApp.Migrations
{
    /// <inheritdoc />
    public partial class AddRejectionReasonToReturnRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ReturnRequests_Orders_OrderId",
                table: "ReturnRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_ReturnRequests_Users_userId",
                table: "ReturnRequests");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Users",
                newName: "userId");

            migrationBuilder.RenameColumn(
                name: "userId",
                table: "ReturnRequests",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_ReturnRequests_userId",
                table: "ReturnRequests",
                newName: "IX_ReturnRequests_UserId");

            migrationBuilder.RenameColumn(
                name: "GenderCategory",
                table: "Products",
                newName: "Category");

            migrationBuilder.AlterColumn<int>(
                name: "OrderId",
                table: "ReturnRequests",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RejectionReason",
                table: "ReturnRequests",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "SellerId",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Products_SellerId",
                table: "Products",
                column: "SellerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Users_SellerId",
                table: "Products",
                column: "SellerId",
                principalTable: "Users",
                principalColumn: "userId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ReturnRequests_Orders_OrderId",
                table: "ReturnRequests",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ReturnRequests_Users_UserId",
                table: "ReturnRequests",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "userId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_Users_SellerId",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK_ReturnRequests_Orders_OrderId",
                table: "ReturnRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_ReturnRequests_Users_UserId",
                table: "ReturnRequests");

            migrationBuilder.DropIndex(
                name: "IX_Products_SellerId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "RejectionReason",
                table: "ReturnRequests");

            migrationBuilder.DropColumn(
                name: "SellerId",
                table: "Products");

            migrationBuilder.RenameColumn(
                name: "userId",
                table: "Users",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "ReturnRequests",
                newName: "userId");

            migrationBuilder.RenameIndex(
                name: "IX_ReturnRequests_UserId",
                table: "ReturnRequests",
                newName: "IX_ReturnRequests_userId");

            migrationBuilder.RenameColumn(
                name: "Category",
                table: "Products",
                newName: "GenderCategory");

            migrationBuilder.AlterColumn<int>(
                name: "OrderId",
                table: "ReturnRequests",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_ReturnRequests_Orders_OrderId",
                table: "ReturnRequests",
                column: "OrderId",
                principalTable: "Orders",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ReturnRequests_Users_userId",
                table: "ReturnRequests",
                column: "userId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
