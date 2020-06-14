using Microsoft.EntityFrameworkCore.Migrations;

namespace AstCaller.Migrations
{
    public partial class AddCallAttempts : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CallAttempts",
                table: "campaignabonent",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CallAttempts",
                table: "campaignabonent");
        }
    }
}
