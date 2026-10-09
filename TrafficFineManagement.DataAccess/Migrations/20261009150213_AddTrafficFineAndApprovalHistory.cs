using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrafficFineManagement.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddTrafficFineAndApprovalHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TrafficFines",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FineNumber = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    ViolationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ViolationType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Location = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    VehicleId = table.Column<int>(type: "int", nullable: false),
                    CreatedByUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrafficFines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TrafficFines_AspNetUsers_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TrafficFines_Vehicles_VehicleId",
                        column: x => x.VehicleId,
                        principalTable: "Vehicles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FineApprovalHistories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TrafficFineId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ActionType = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    FromStatus = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    ToStatus = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    Note = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FineApprovalHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FineApprovalHistories_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FineApprovalHistories_TrafficFines_TrafficFineId",
                        column: x => x.TrafficFineId,
                        principalTable: "TrafficFines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FineApprovalHistories_TrafficFineId_CreatedAt",
                table: "FineApprovalHistories",
                columns: new[] { "TrafficFineId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_FineApprovalHistories_UserId",
                table: "FineApprovalHistories",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_TrafficFines_CreatedByUserId",
                table: "TrafficFines",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_TrafficFines_FineNumber",
                table: "TrafficFines",
                column: "FineNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TrafficFines_Status",
                table: "TrafficFines",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_TrafficFines_VehicleId",
                table: "TrafficFines",
                column: "VehicleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FineApprovalHistories");

            migrationBuilder.DropTable(
                name: "TrafficFines");
        }
    }
}
