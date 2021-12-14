using Microsoft.EntityFrameworkCore.Migrations;

namespace Doctors.Migrations
{
    public partial class RemoveUserCompany3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Ownerd",
                table: "Companies");

            migrationBuilder.AddColumn<string>(
                name: "OwnerId",
                table: "Companies",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "Companies");

            migrationBuilder.AddColumn<string>(
                name: "Ownerd",
                table: "Companies",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
