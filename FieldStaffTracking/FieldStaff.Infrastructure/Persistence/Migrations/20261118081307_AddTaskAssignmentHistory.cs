using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FieldStaff.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddTaskAssignmentHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TaskStatusHistory_Users_UserId",
                table: "TaskStatusHistory");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "TaskStatusHistory",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "TaskAssignmentHistory",
                columns: table => new
                {
                    TaskAssignmentHistoryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ServiceTaskId = table.Column<int>(type: "int", nullable: false),
                    OldAssignedStaffId = table.Column<int>(type: "int", nullable: true),
                    NewAssignedStaffId = table.Column<int>(type: "int", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<int>(type: "int", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskAssignmentHistory", x => x.TaskAssignmentHistoryId);
                    table.ForeignKey(
                        name: "FK_TaskAssignmentHistory_ServiceTasks_ServiceTaskId",
                        column: x => x.ServiceTaskId,
                        principalTable: "ServiceTasks",
                        principalColumn: "ServiceTaskId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TaskAssignmentHistory_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TaskAssignmentHistory_Users_NewAssignedStaffId",
                        column: x => x.NewAssignedStaffId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TaskAssignmentHistory_Users_OldAssignedStaffId",
                        column: x => x.OldAssignedStaffId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TaskAssignmentHistory_Users_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TaskAssignmentHistory_CreatedBy",
                table: "TaskAssignmentHistory",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_TaskAssignmentHistory_NewAssignedStaffId",
                table: "TaskAssignmentHistory",
                column: "NewAssignedStaffId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskAssignmentHistory_OldAssignedStaffId",
                table: "TaskAssignmentHistory",
                column: "OldAssignedStaffId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskAssignmentHistory_ServiceTaskId",
                table: "TaskAssignmentHistory",
                column: "ServiceTaskId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskAssignmentHistory_UpdatedBy",
                table: "TaskAssignmentHistory",
                column: "UpdatedBy");

            migrationBuilder.AddForeignKey(
                name: "FK_TaskStatusHistory_Users_UserId",
                table: "TaskStatusHistory",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TaskStatusHistory_Users_UserId",
                table: "TaskStatusHistory");

            migrationBuilder.DropTable(
                name: "TaskAssignmentHistory");

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "TaskStatusHistory",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_TaskStatusHistory_Users_UserId",
                table: "TaskStatusHistory",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId");
        }
    }
}
