using Microsoft.EntityFrameworkCore.Migrations;

namespace CertificateManagementSystem.Data.Migrations
{
    public partial class Methodicmodification : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RegistrationNumber",
                table: "VerificationMethodics");

            migrationBuilder.AddColumn<string>(
                name: "RegistrationNumber",
                table: "Devices",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RegistrationNumber",
                table: "Devices");

            migrationBuilder.AddColumn<string>(
                name: "RegistrationNumber",
                table: "VerificationMethodics",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
