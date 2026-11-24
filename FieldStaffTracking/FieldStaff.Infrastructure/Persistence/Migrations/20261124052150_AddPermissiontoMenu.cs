using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FieldStaff.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddPermissiontoMenu : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PermissionId",
                table: "Menus",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Menus_PermissionId",
                table: "Menus",
                column: "PermissionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Menus_Permissions_PermissionId",
                table: "Menus",
                column: "PermissionId",
                principalTable: "Permissions",
                principalColumn: "PermissionId",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Menus_Permissions_PermissionId",
                table: "Menus");

            migrationBuilder.DropIndex(
                name: "IX_Menus_PermissionId",
                table: "Menus");

            migrationBuilder.DropColumn(
                name: "PermissionId",
                table: "Menus");
        }
    }
}
