using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace AstCaller.Migrations
{
    public partial class AddCallHistory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "campaignabonent",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddColumn<DateTime>(
                name: "CallStartDate",
                table: "campaignabonent",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "callstatus",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    StatusName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_callstatus", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "campaignabonenthistory",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    CampaignAbonentId = table.Column<int>(nullable: false),
                    Status = table.Column<int>(nullable: true),
                    CallDate = table.Column<DateTime>(nullable: false),
                    Reason = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_campaignabonenthistory", x => x.Id);
                    table.ForeignKey(
                        name: "fk_campaignabonenthistory_campaignabonent",
                        column: x => x.CampaignAbonentId,
                        principalTable: "campaignabonent",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_campaignabonenthistory_callstatus",
                        column: x => x.Status,
                        principalTable: "callstatus",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_campaignabonent_Status",
                table: "campaignabonent",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_campaignabonenthistory_CampaignAbonentId",
                table: "campaignabonenthistory",
                column: "CampaignAbonentId");

            migrationBuilder.CreateIndex(
                name: "IX_campaignabonenthistory_Status",
                table: "campaignabonenthistory",
                column: "Status");

            migrationBuilder.AddForeignKey(
                name: "fk_campaignabonent_callstatus",
                table: "campaignabonent",
                column: "Status",
                principalTable: "callstatus",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_campaignabonent_callstatus",
                table: "campaignabonent");

            migrationBuilder.DropTable(
                name: "campaignabonenthistory");

            migrationBuilder.DropTable(
                name: "callstatus");

            migrationBuilder.DropIndex(
                name: "IX_campaignabonent_Status",
                table: "campaignabonent");

            migrationBuilder.DropColumn(
                name: "CallStartDate",
                table: "campaignabonent");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "campaignabonent",
                nullable: false,
                oldClrType: typeof(int),
                oldNullable: true);
        }
    }
}
