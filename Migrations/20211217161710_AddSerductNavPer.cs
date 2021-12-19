using Microsoft.EntityFrameworkCore.Migrations;

namespace Doctors.Migrations
{
    public partial class AddSerductNavPer : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SerductImages_Serducts_SerductId",
                table: "SerductImages");

            migrationBuilder.DropForeignKey(
                name: "FK_Serducts_Companies_CompanyId",
                table: "Serducts");

            migrationBuilder.RenameColumn(
                name: "COmpanyImageType",
                table: "CompanyImages",
                newName: "CompanyImageType");

            migrationBuilder.AlterColumn<string>(
                name: "CompanyId",
                table: "Serducts",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "SerductId",
                table: "SerductImages",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_SerductImages_Serducts_SerductId",
                table: "SerductImages",
                column: "SerductId",
                principalTable: "Serducts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Serducts_Companies_CompanyId",
                table: "Serducts",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SerductImages_Serducts_SerductId",
                table: "SerductImages");

            migrationBuilder.DropForeignKey(
                name: "FK_Serducts_Companies_CompanyId",
                table: "Serducts");

            migrationBuilder.RenameColumn(
                name: "CompanyImageType",
                table: "CompanyImages",
                newName: "COmpanyImageType");

            migrationBuilder.AlterColumn<string>(
                name: "CompanyId",
                table: "Serducts",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "SerductId",
                table: "SerductImages",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AddForeignKey(
                name: "FK_SerductImages_Serducts_SerductId",
                table: "SerductImages",
                column: "SerductId",
                principalTable: "Serducts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Serducts_Companies_CompanyId",
                table: "Serducts",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
