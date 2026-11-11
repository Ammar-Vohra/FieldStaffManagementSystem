using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FieldStaff.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SecondCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AuditLogs_TaskStatuses_StatusTaskTaskStatusId",
                table: "AuditLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_AuditLogs_Users_CreatedByUserUserId",
                table: "AuditLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_ServiceTasks_Users_UserId",
                table: "ServiceTasks");

            migrationBuilder.DropIndex(
                name: "IX_ServiceTasks_UserId",
                table: "ServiceTasks");

            migrationBuilder.DropIndex(
                name: "IX_AuditLogs_CreatedByUserUserId",
                table: "AuditLogs");

            migrationBuilder.DropIndex(
                name: "IX_AuditLogs_StatusTaskTaskStatusId",
                table: "AuditLogs");

            migrationBuilder.DropColumn(
                name: "CreateByUser",
                table: "ServiceTasks");

            migrationBuilder.DropColumn(
                name: "UpdatedByUser",
                table: "ServiceTasks");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "ServiceTasks");

            migrationBuilder.DropColumn(
                name: "CreatedByUserUserId",
                table: "AuditLogs");

            migrationBuilder.DropColumn(
                name: "StatusTaskTaskStatusId",
                table: "AuditLogs");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CreateByUser",
                table: "ServiceTasks",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UpdatedByUser",
                table: "ServiceTasks",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "ServiceTasks",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CreatedByUserUserId",
                table: "AuditLogs",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "StatusTaskTaskStatusId",
                table: "AuditLogs",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ServiceTasks_UserId",
                table: "ServiceTasks",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_CreatedByUserUserId",
                table: "AuditLogs",
                column: "CreatedByUserUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_StatusTaskTaskStatusId",
                table: "AuditLogs",
                column: "StatusTaskTaskStatusId");

            migrationBuilder.AddForeignKey(
                name: "FK_AuditLogs_TaskStatuses_StatusTaskTaskStatusId",
                table: "AuditLogs",
                column: "StatusTaskTaskStatusId",
                principalTable: "TaskStatuses",
                principalColumn: "TaskStatusId");

            migrationBuilder.AddForeignKey(
                name: "FK_AuditLogs_Users_CreatedByUserUserId",
                table: "AuditLogs",
                column: "CreatedByUserUserId",
                principalTable: "Users",
                principalColumn: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_ServiceTasks_Users_UserId",
                table: "ServiceTasks",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId");
        }
    }
}
