using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GHotel.Persistance.Migrations
{
    public partial class AddMyRoleTable1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MyRoleMyUser_Roles_RolesName",
                table: "MyRoleMyUser");

            migrationBuilder.DropForeignKey(
                name: "FK_MyRoleMyUser_Users_UsersId",
                table: "MyRoleMyUser");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MyRoleMyUser",
                table: "MyRoleMyUser");

            migrationBuilder.RenameTable(
                name: "MyRoleMyUser",
                newName: "UserRoles");

            migrationBuilder.RenameIndex(
                name: "IX_MyRoleMyUser_UsersId",
                table: "UserRoles",
                newName: "IX_UserRoles_UsersId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserRoles",
                table: "UserRoles",
                columns: new[] { "RolesName", "UsersId" });

            migrationBuilder.AddForeignKey(
                name: "FK_UserRoles_Roles_RolesName",
                table: "UserRoles",
                column: "RolesName",
                principalTable: "Roles",
                principalColumn: "Name",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserRoles_Users_UsersId",
                table: "UserRoles",
                column: "UsersId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserRoles_Roles_RolesName",
                table: "UserRoles");

            migrationBuilder.DropForeignKey(
                name: "FK_UserRoles_Users_UsersId",
                table: "UserRoles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserRoles",
                table: "UserRoles");

            migrationBuilder.RenameTable(
                name: "UserRoles",
                newName: "MyRoleMyUser");

            migrationBuilder.RenameIndex(
                name: "IX_UserRoles_UsersId",
                table: "MyRoleMyUser",
                newName: "IX_MyRoleMyUser_UsersId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MyRoleMyUser",
                table: "MyRoleMyUser",
                columns: new[] { "RolesName", "UsersId" });

            migrationBuilder.AddForeignKey(
                name: "FK_MyRoleMyUser_Roles_RolesName",
                table: "MyRoleMyUser",
                column: "RolesName",
                principalTable: "Roles",
                principalColumn: "Name",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MyRoleMyUser_Users_UsersId",
                table: "MyRoleMyUser",
                column: "UsersId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
