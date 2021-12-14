using Microsoft.EntityFrameworkCore.Migrations;

namespace Doctors.Migrations
{
    public partial class RemoveUserCompany2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Ownerd",
                table: "Companies",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Ownerd",
                table: "Companies");
        }
    }
}
