using Microsoft.EntityFrameworkCore.Migrations;

namespace Doctors.Migrations
{
    public partial class AddVIrtualCompanyImage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Companies_CompanyImages_LogoImageId",
                table: "Companies");

            migrationBuilder.DropForeignKey(
                name: "FK_Companies_CompanyImages_ProfileImageId",
                table: "Companies");

            migrationBuilder.DropIndex(
                name: "IX_Companies_LogoImageId",
                table: "Companies");

            migrationBuilder.DropIndex(
                name: "IX_Companies_ProfileImageId",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "LogoImageId",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "ProfileImageId",
                table: "Companies");

            migrationBuilder.AddColumn<string>(
                name: "CompanyId",
                table: "CompanyImages",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CompanyImages_CompanyId",
                table: "CompanyImages",
                column: "CompanyId");

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyImages_Companies_CompanyId",
                table: "CompanyImages",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CompanyImages_Companies_CompanyId",
                table: "CompanyImages");

            migrationBuilder.DropIndex(
                name: "IX_CompanyImages_CompanyId",
                table: "CompanyImages");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "CompanyImages");

            migrationBuilder.AddColumn<string>(
                name: "LogoImageId",
                table: "Companies",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProfileImageId",
                table: "Companies",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Companies_LogoImageId",
                table: "Companies",
                column: "LogoImageId");

            migrationBuilder.CreateIndex(
                name: "IX_Companies_ProfileImageId",
                table: "Companies",
                column: "ProfileImageId");

            migrationBuilder.AddForeignKey(
                name: "FK_Companies_CompanyImages_LogoImageId",
                table: "Companies",
                column: "LogoImageId",
                principalTable: "CompanyImages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Companies_CompanyImages_ProfileImageId",
                table: "Companies",
                column: "ProfileImageId",
                principalTable: "CompanyImages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
