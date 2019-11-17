using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AstCaller.Migrations
{
    public partial class AddTimeConditions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "campaignabonent",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "UniqueId",
                table: "campaignabonent",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "campaignschedule",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CampaignId = table.Column<int>(nullable: false),
                    DateStart = table.Column<DateTime>(nullable: false),
                    DateEnd = table.Column<DateTime>(nullable: false),
                    TimeStart = table.Column<int>(nullable: false),
                    TimeEnd = table.Column<int>(nullable: false),
                    DaysOfWeek = table.Column<int>(nullable: false),
                    ModifierId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_campaignschedule", x => x.Id);
                    table.ForeignKey(
                        name: "fk_campaignschedule_campaign",
                        column: x => x.CampaignId,
                        principalTable: "campaign",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_campaignschedule_modifier",
                        column: x => x.ModifierId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_campaignschedule_CampaignId",
                table: "campaignschedule",
                column: "CampaignId");

            migrationBuilder.CreateIndex(
                name: "IX_campaignschedule_ModifierId",
                table: "campaignschedule",
                column: "ModifierId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "campaignschedule");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "campaignabonent");

            migrationBuilder.DropColumn(
                name: "UniqueId",
                table: "campaignabonent");
        }
    }
}
