﻿// <auto-generated />
using System;
using AstCaller.Models.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace AstCaller.Migrations
{
    [DbContext(typeof(MainContext))]
    partial class MainContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.6-servicing-10079")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("AstCaller.Models.Domain.AsteriskExtension", b =>
                {
                    b.Property<string>("Extension")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("Disabled");

                    b.Property<string>("ExtensionCode");

                    b.Property<int>("ModifierId");

                    b.Property<string>("Title");

                    b.HasKey("Extension")
                        .HasName("pk_asteriskextension");

                    b.HasIndex("ModifierId");

                    b.ToTable("asteriskextension");
                });

            modelBuilder.Entity("AstCaller.Models.Domain.CallStatus", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("StatusName");

                    b.HasKey("Id")
                        .HasName("pk_callstatus");

                    b.ToTable("callstatus");
                });

            modelBuilder.Entity("AstCaller.Models.Domain.Campaign", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AbonentsCount");

                    b.Property<string>("AbonentsFileName")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("Extension");

                    b.Property<DateTime>("Modified");

                    b.Property<int>("ModifierId");

                    b.Property<string>("Name")
                        .HasColumnType("varchar(500)");

                    b.Property<int>("Status");

                    b.Property<string>("VoiceFileName")
                        .HasColumnType("varchar(255)");

                    b.HasKey("Id");

                    b.HasIndex("Extension");

                    b.HasIndex("ModifierId");

                    b.ToTable("campaign");
                });

            modelBuilder.Entity("AstCaller.Models.Domain.CampaignAbonent", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("CallInfo");

                    b.Property<DateTime?>("CallStartDate");

                    b.Property<int>("CampaignId");

                    b.Property<bool>("HasErrors");

                    b.Property<DateTime>("Modified");

                    b.Property<int?>("ModifierId");

                    b.Property<string>("Phone")
                        .HasColumnType("varchar(500)");

                    b.Property<int?>("Status");

                    b.Property<Guid>("UniqueId");

                    b.HasKey("Id")
                        .HasName("pk_campaignabonent");

                    b.HasIndex("CampaignId");

                    b.HasIndex("ModifierId");

                    b.HasIndex("Status");

                    b.ToTable("campaignabonent");
                });

            modelBuilder.Entity("AstCaller.Models.Domain.CampaignAbonentHistory", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CallDate");

                    b.Property<int>("CampaignAbonentId");

                    b.Property<string>("Reason");

                    b.Property<int?>("Status");

                    b.HasKey("Id")
                        .HasName("pk_campaignabonenthistory");

                    b.HasIndex("CampaignAbonentId");

                    b.HasIndex("Status");

                    b.ToTable("campaignabonenthistory");
                });

            modelBuilder.Entity("AstCaller.Models.Domain.CampaignSchedule", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("CampaignId");

                    b.Property<DateTime>("DateEnd");

                    b.Property<DateTime>("DateStart");

                    b.Property<int>("DaysOfWeek");

                    b.Property<int>("ModifierId");

                    b.Property<int>("TimeEnd");

                    b.Property<int>("TimeStart");

                    b.HasKey("Id")
                        .HasName("pk_campaignschedule");

                    b.HasIndex("CampaignId");

                    b.HasIndex("ModifierId");

                    b.ToTable("campaignschedule");
                });

            modelBuilder.Entity("AstCaller.Models.Domain.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Fullname")
                        .HasColumnType("varchar(500)");

                    b.Property<string>("Login")
                        .IsRequired()
                        .HasColumnType("varchar(200)");

                    b.Property<string>("Password")
                        .HasColumnType("varchar(200)");

                    b.HasKey("Id");

                    b.ToTable("users");
                });

            modelBuilder.Entity("AstCaller.Models.Domain.AsteriskExtension", b =>
                {
                    b.HasOne("AstCaller.Models.Domain.User", "Modifier")
                        .WithMany("AsteriskExtensions")
                        .HasForeignKey("ModifierId")
                        .HasConstraintName("fk_asteriskextension_modifier")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("AstCaller.Models.Domain.Campaign", b =>
                {
                    b.HasOne("AstCaller.Models.Domain.AsteriskExtension", "AsteriskExtension")
                        .WithMany("Campaigns")
                        .HasForeignKey("Extension")
                        .HasConstraintName("fk_campaign_extension");

                    b.HasOne("AstCaller.Models.Domain.User", "Modifier")
                        .WithMany("Campaigns")
                        .HasForeignKey("ModifierId")
                        .HasConstraintName("fk_campaign_modifier")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("AstCaller.Models.Domain.CampaignAbonent", b =>
                {
                    b.HasOne("AstCaller.Models.Domain.Campaign", "Campaign")
                        .WithMany("CampaignAbonents")
                        .HasForeignKey("CampaignId")
                        .HasConstraintName("fk_campaignabonent_campaign")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("AstCaller.Models.Domain.User", "Modifier")
                        .WithMany("CampaignAbonents")
                        .HasForeignKey("ModifierId")
                        .HasConstraintName("fk_campaignabonent_modifier");

                    b.HasOne("AstCaller.Models.Domain.CallStatus", "CallStatus")
                        .WithMany("CampaignAbonents")
                        .HasForeignKey("Status")
                        .HasConstraintName("fk_campaignabonent_callstatus");
                });

            modelBuilder.Entity("AstCaller.Models.Domain.CampaignAbonentHistory", b =>
                {
                    b.HasOne("AstCaller.Models.Domain.CampaignAbonent", "CampaignAbonent")
                        .WithMany("CampaignAbonentHistories")
                        .HasForeignKey("CampaignAbonentId")
                        .HasConstraintName("fk_campaignabonenthistory_campaignabonent")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("AstCaller.Models.Domain.CallStatus", "CallStatus")
                        .WithMany("CampaignAbonentHistories")
                        .HasForeignKey("Status")
                        .HasConstraintName("fk_campaignabonenthistory_callstatus");
                });

            modelBuilder.Entity("AstCaller.Models.Domain.CampaignSchedule", b =>
                {
                    b.HasOne("AstCaller.Models.Domain.Campaign", "Campaign")
                        .WithMany("CampaignSchedules")
                        .HasForeignKey("CampaignId")
                        .HasConstraintName("fk_campaignschedule_campaign")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("AstCaller.Models.Domain.User", "Modifier")
                        .WithMany("CampaignSchedules")
                        .HasForeignKey("ModifierId")
                        .HasConstraintName("fk_campaignschedule_modifier")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
