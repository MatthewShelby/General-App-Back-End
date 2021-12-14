using Microsoft.EntityFrameworkCore.Migrations;

namespace Doctors.Migrations
{
    public partial class ChangeUserCompany : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UsersCompanys_Companies_CompanyId",
                table: "UsersCompanys");

            migrationBuilder.DropForeignKey(
                name: "FK_UsersCompanys_AspNetUsers_UserId",
                table: "UsersCompanys");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UsersCompanys",
                table: "UsersCompanys");

            migrationBuilder.DropIndex(
                name: "IX_UsersCompanys_CompanyId",
                table: "UsersCompanys");

            migrationBuilder.DropIndex(
                name: "IX_UsersCompanys_UserId",
                table: "UsersCompanys");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "UsersCompanys");

            migrationBuilder.RenameTable(
                name: "UsersCompanys",
                newName: "UsersCompany");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "UsersCompany",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CompanyId",
                table: "UsersCompany",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "UsersCompany",
                newName: "UsersCompanys");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "UsersCompanys",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CompanyId",
                table: "UsersCompanys",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Id",
                table: "UsersCompanys",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UsersCompanys",
                table: "UsersCompanys",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_UsersCompanys_CompanyId",
                table: "UsersCompanys",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_UsersCompanys_UserId",
                table: "UsersCompanys",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_UsersCompanys_Companies_CompanyId",
                table: "UsersCompanys",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UsersCompanys_AspNetUsers_UserId",
                table: "UsersCompanys",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
