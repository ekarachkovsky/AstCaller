using Microsoft.EntityFrameworkCore.Migrations;

namespace AstCaller.Migrations
{
    public partial class LineLimitForSchedule : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LineLimit",
                table: "campaign",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LineLimit",
                table: "campaign");
        }
    }
}
