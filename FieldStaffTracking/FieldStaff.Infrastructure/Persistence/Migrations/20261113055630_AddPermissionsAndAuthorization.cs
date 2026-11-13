using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FieldStaff.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPermissionsAndAuthorization : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Permissions_Menus_MenuId",
                table: "Permissions");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskStatusHistory_Users_ChangedByUserUserId",
                table: "TaskStatusHistory");

            migrationBuilder.DropIndex(
                name: "IX_UserPermissions_UserId",
                table: "UserPermissions");

            migrationBuilder.DropIndex(
                name: "IX_TaskStatusHistory_ChangedByUserUserId",
                table: "TaskStatusHistory");

            migrationBuilder.DropIndex(
                name: "IX_RolePermissions_RoleId_PermissionId",
                table: "RolePermissions");

            migrationBuilder.DropIndex(
                name: "IX_Permissions_MenuId",
                table: "Permissions");

            migrationBuilder.DropIndex(
                name: "IX_Permissions_PermissionCode",
                table: "Permissions");

            migrationBuilder.DropColumn(
                name: "ChangedByUserUserId",
                table: "TaskStatusHistory");

            migrationBuilder.DropColumn(
                name: "MenuId",
                table: "Permissions");

            migrationBuilder.AddColumn<bool>(
                name: "IsGranted",
                table: "UserPermissions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "TaskStatusHistory",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "RolePermissions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "RolePermissions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Icon",
                table: "Menus",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ParentMenuId",
                table: "Menus",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SortOrder",
                table: "Menus",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "MenuPermissions",
                columns: table => new
                {
                    MenuPermissionId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MenuId = table.Column<int>(type: "int", nullable: false),
                    PermissionId = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedBy = table.Column<int>(type: "int", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MenuPermissions", x => x.MenuPermissionId);
                    table.ForeignKey(
                        name: "FK_MenuPermissions_Menus_MenuId",
                        column: x => x.MenuId,
                        principalTable: "Menus",
                        principalColumn: "MenuId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MenuPermissions_Permissions_PermissionId",
                        column: x => x.PermissionId,
                        principalTable: "Permissions",
                        principalColumn: "PermissionId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MenuPermissions_Users_CreatedBy",
                        column: x => x.CreatedBy,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MenuPermissions_Users_UpdatedBy",
                        column: x => x.UpdatedBy,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserPermissions_UserId_PermissionId",
                table: "UserPermissions",
                columns: new[] { "UserId", "PermissionId" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_TaskStatusHistory_UserId",
                table: "TaskStatusHistory",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_RoleId_PermissionId",
                table: "RolePermissions",
                columns: new[] { "RoleId", "PermissionId" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_PermissionCode",
                table: "Permissions",
                column: "PermissionCode",
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_Menus_ParentMenuId",
                table: "Menus",
                column: "ParentMenuId");

            migrationBuilder.CreateIndex(
                name: "IX_MenuPermissions_CreatedBy",
                table: "MenuPermissions",
                column: "CreatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_MenuPermissions_MenuId_PermissionId",
                table: "MenuPermissions",
                columns: new[] { "MenuId", "PermissionId" },
                unique: true,
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_MenuPermissions_PermissionId",
                table: "MenuPermissions",
                column: "PermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_MenuPermissions_UpdatedBy",
                table: "MenuPermissions",
                column: "UpdatedBy");

            migrationBuilder.AddForeignKey(
                name: "FK_Menus_Menus_ParentMenuId",
                table: "Menus",
                column: "ParentMenuId",
                principalTable: "Menus",
                principalColumn: "MenuId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TaskStatusHistory_Users_UserId",
                table: "TaskStatusHistory",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Menus_Menus_ParentMenuId",
                table: "Menus");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskStatusHistory_Users_UserId",
                table: "TaskStatusHistory");

            migrationBuilder.DropTable(
                name: "MenuPermissions");

            migrationBuilder.DropIndex(
                name: "IX_UserPermissions_UserId_PermissionId",
                table: "UserPermissions");

            migrationBuilder.DropIndex(
                name: "IX_TaskStatusHistory_UserId",
                table: "TaskStatusHistory");

            migrationBuilder.DropIndex(
                name: "IX_RolePermissions_RoleId_PermissionId",
                table: "RolePermissions");

            migrationBuilder.DropIndex(
                name: "IX_Permissions_PermissionCode",
                table: "Permissions");

            migrationBuilder.DropIndex(
                name: "IX_Menus_ParentMenuId",
                table: "Menus");

            migrationBuilder.DropColumn(
                name: "IsGranted",
                table: "UserPermissions");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "TaskStatusHistory");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "RolePermissions");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "RolePermissions");

            migrationBuilder.DropColumn(
                name: "Icon",
                table: "Menus");

            migrationBuilder.DropColumn(
                name: "ParentMenuId",
                table: "Menus");

            migrationBuilder.DropColumn(
                name: "SortOrder",
                table: "Menus");

            migrationBuilder.AddColumn<int>(
                name: "ChangedByUserUserId",
                table: "TaskStatusHistory",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MenuId",
                table: "Permissions",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserPermissions_UserId",
                table: "UserPermissions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskStatusHistory_ChangedByUserUserId",
                table: "TaskStatusHistory",
                column: "ChangedByUserUserId");

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_RoleId_PermissionId",
                table: "RolePermissions",
                columns: new[] { "RoleId", "PermissionId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_MenuId",
                table: "Permissions",
                column: "MenuId");

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_PermissionCode",
                table: "Permissions",
                column: "PermissionCode",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Permissions_Menus_MenuId",
                table: "Permissions",
                column: "MenuId",
                principalTable: "Menus",
                principalColumn: "MenuId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_TaskStatusHistory_Users_ChangedByUserUserId",
                table: "TaskStatusHistory",
                column: "ChangedByUserUserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
