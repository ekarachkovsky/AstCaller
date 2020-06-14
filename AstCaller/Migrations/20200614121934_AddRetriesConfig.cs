using Microsoft.EntityFrameworkCore.Migrations;

namespace AstCaller.Migrations
{
    public partial class AddRetriesConfig : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Retries",
                table: "campaign",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Retries",
                table: "campaign");
        }
    }
}
