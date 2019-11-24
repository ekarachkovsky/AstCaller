using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AstCaller.Migrations
{
    public partial class AddExtensions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CallInfo",
                table: "campaignabonent",
                nullable: true);

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

            migrationBuilder.AddColumn<string>(
                name: "Extension",
                table: "campaign",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "asteriskextension",
                columns: table => new
                {
                    Extension = table.Column<string>(nullable: false),
                    Title = table.Column<string>(nullable: true),
                    ExtensionCode = table.Column<string>(nullable: true),
                    ModifierId = table.Column<int>(nullable: false),
                    Disabled = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_asteriskextension", x => x.Extension);
                    table.ForeignKey(
                        name: "fk_asteriskextension_modifier",
                        column: x => x.ModifierId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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
                name: "IX_campaign_Extension",
                table: "campaign",
                column: "Extension");

            migrationBuilder.CreateIndex(
                name: "IX_asteriskextension_ModifierId",
                table: "asteriskextension",
                column: "ModifierId");

            migrationBuilder.CreateIndex(
                name: "IX_campaignschedule_CampaignId",
                table: "campaignschedule",
                column: "CampaignId");

            migrationBuilder.CreateIndex(
                name: "IX_campaignschedule_ModifierId",
                table: "campaignschedule",
                column: "ModifierId");

            migrationBuilder.AddForeignKey(
                name: "fk_campaign_extension",
                table: "campaign",
                column: "Extension",
                principalTable: "asteriskextension",
                principalColumn: "Extension",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_campaign_extension",
                table: "campaign");

            migrationBuilder.DropTable(
                name: "asteriskextension");

            migrationBuilder.DropTable(
                name: "campaignschedule");

            migrationBuilder.DropIndex(
                name: "IX_campaign_Extension",
                table: "campaign");

            migrationBuilder.DropColumn(
                name: "CallInfo",
                table: "campaignabonent");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "campaignabonent");

            migrationBuilder.DropColumn(
                name: "UniqueId",
                table: "campaignabonent");

            migrationBuilder.DropColumn(
                name: "Extension",
                table: "campaign");
        }
    }
}
