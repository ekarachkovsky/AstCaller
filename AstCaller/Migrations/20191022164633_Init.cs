using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AstCaller.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Fullname = table.Column<string>(type: "varchar(500)", nullable: true),
                    Login = table.Column<string>(type: "varchar(200)", nullable: false),
                    Password = table.Column<string>(type: "varchar(200)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "campaign",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(500)", nullable: true),
                    Status = table.Column<int>(nullable: false),
                    AbonentsCount = table.Column<int>(nullable: false),
                    AbonentsFileName = table.Column<string>(type: "varchar(255)", nullable: true),
                    VoiceFileName = table.Column<string>(type: "varchar(255)", nullable: true),
                    ModifierId = table.Column<int>(nullable: false),
                    Modified = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_campaign", x => x.Id);
                    table.ForeignKey(
                        name: "fk_campaign_modifier",
                        column: x => x.ModifierId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "campaignabonent",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CampaignId = table.Column<int>(nullable: false),
                    Phone = table.Column<string>(type: "varchar(500)", nullable: true),
                    HasErrors = table.Column<bool>(nullable: false),
                    ModifierId = table.Column<int>(nullable: true),
                    Modified = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_campaignabonent", x => x.Id);
                    table.ForeignKey(
                        name: "fk_campaignabonent_campaign",
                        column: x => x.CampaignId,
                        principalTable: "campaign",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_campaignabonent_modifier",
                        column: x => x.ModifierId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_campaign_ModifierId",
                table: "campaign",
                column: "ModifierId");

            migrationBuilder.CreateIndex(
                name: "IX_campaignabonent_CampaignId",
                table: "campaignabonent",
                column: "CampaignId");

            migrationBuilder.CreateIndex(
                name: "IX_campaignabonent_ModifierId",
                table: "campaignabonent",
                column: "ModifierId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "campaignabonent");

            migrationBuilder.DropTable(
                name: "campaign");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
