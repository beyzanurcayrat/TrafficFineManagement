using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrafficFineManagement.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddFineRecipientPersonnel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FineForUserId",
                table: "TrafficFines",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.Sql(
                "UPDATE [TrafficFines] SET [FineForUserId] = [CreatedByUserId] WHERE [FineForUserId] IS NULL");

            migrationBuilder.AlterColumn<string>(
                name: "FineForUserId",
                table: "TrafficFines",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TrafficFines_FineForUserId",
                table: "TrafficFines",
                column: "FineForUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_TrafficFines_AspNetUsers_FineForUserId",
                table: "TrafficFines",
                column: "FineForUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TrafficFines_AspNetUsers_FineForUserId",
                table: "TrafficFines");

            migrationBuilder.DropIndex(
                name: "IX_TrafficFines_FineForUserId",
                table: "TrafficFines");

            migrationBuilder.DropColumn(
                name: "FineForUserId",
                table: "TrafficFines");
        }
    }
}
