using Microsoft.EntityFrameworkCore.Migrations;

namespace AstCaller.Migrations
{
    public partial class AddCloneFeature : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ClonedFromId",
                table: "campaign",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_campaign_ClonedFromId",
                table: "campaign",
                column: "ClonedFromId");

            migrationBuilder.AddForeignKey(
                name: "fk_campaign_campaign_clone",
                table: "campaign",
                column: "ClonedFromId",
                principalTable: "campaign",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_campaign_campaign_clone",
                table: "campaign");

            migrationBuilder.DropIndex(
                name: "IX_campaign_ClonedFromId",
                table: "campaign");

            migrationBuilder.DropColumn(
                name: "ClonedFromId",
                table: "campaign");
        }
    }
}
