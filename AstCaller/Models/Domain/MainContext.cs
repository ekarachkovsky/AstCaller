﻿using Microsoft.EntityFrameworkCore;

namespace AstCaller.Models.Domain
{
    public class MainContext : DbContext
    {
        public MainContext(DbContextOptions<MainContext> options) : base(options)
        {

        }

        public DbSet<User> Users { get; set; }
        public DbSet<Campaign> Campaigns { get; set; }
        public DbSet<CampaignAbonent> CampaignAbonents { get; set; }
        public DbSet<CampaignAbonentHistory> CampaignAbonentHistories { get; set; }
        public DbSet<CampaignSchedule> CampaignSchedules { get; set; }
        public DbSet<AsteriskExtension> AsteriskExtensions { get; set; }
        public DbSet<CallStatus> CallStatuses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("users");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Login).HasColumnType("varchar(200)").IsRequired();
                entity.Property(e => e.Fullname).HasColumnType("varchar(500)");
                entity.Property(e => e.Password).HasColumnType("varchar(200)");
            });
            modelBuilder.Entity<Campaign>(entity =>
            {
                entity.ToTable("campaign");
                entity.HasKey(e => e.Id);
                entity.HasOne(p => p.Modifier)
                    .WithMany(u => u.Campaigns)
                    .HasForeignKey(p => p.ModifierId)
                    .HasConstraintName("fk_campaign_modifier");
                entity.Property(p => p.Name).HasColumnType("varchar(500)");
                entity.Property(p => p.AbonentsFileName).HasColumnType("varchar(255)");
                entity.Property(p => p.VoiceFileName).HasColumnType("varchar(255)");
                entity.HasOne(p => p.ClonedFrom)
                    .WithMany(p=>p.Clones)
                    .HasForeignKey(p=>p.ClonedFromId)
                    .HasConstraintName("fk_campaign_campaign_clone");
            });

            modelBuilder.Entity<CampaignAbonent>(entity =>
            {
                entity.ToTable("campaignabonent");
                entity.HasKey(e => e.Id)
                    .HasName("pk_campaignabonent");
                entity.Property(p => p.Phone).HasColumnType("varchar(500)");
                entity.Property(p => p.HasErrors);
                entity.HasOne(p => p.Campaign)
                    .WithMany(c => c.CampaignAbonents)
                    .HasForeignKey(c => c.CampaignId)
                    .HasConstraintName("fk_campaignabonent_campaign");
                entity.HasOne(p => p.Modifier)
                    .WithMany(u => u.CampaignAbonents)
                    .HasForeignKey(p => p.ModifierId)
                    .HasConstraintName("fk_campaignabonent_modifier");

                entity.HasOne(p => p.CallStatus)
                    .WithMany(c => c.CampaignAbonents)
                    .HasForeignKey(c => c.Status)
                    .IsRequired(false)
                    .HasConstraintName("fk_campaignabonent_callstatus");
            });

            modelBuilder.Entity<CampaignSchedule>(entity =>
            {
                entity.ToTable("campaignschedule");
                entity.HasKey(e => e.Id)
                    .HasName("pk_campaignschedule");

                entity.HasOne(p => p.Campaign)
                    .WithMany(c => c.CampaignSchedules)
                    .HasForeignKey(c => c.CampaignId)
                    .HasConstraintName("fk_campaignschedule_campaign");

                entity.HasOne(p => p.Modifier)
                    .WithMany(u => u.CampaignSchedules)
                    .HasForeignKey(c => c.ModifierId)
                    .HasConstraintName("fk_campaignschedule_modifier");
            });

            modelBuilder.Entity<AsteriskExtension>(entity =>
            {
                entity.ToTable("asteriskextension");
                entity.HasKey(e => e.Extension)
                    .HasName("pk_asteriskextension");

                entity.HasOne(p => p.Modifier)
                    .WithMany(u => u.AsteriskExtensions)
                    .HasForeignKey(p => p.ModifierId)
                    .HasConstraintName("fk_asteriskextension_modifier");

                entity.HasMany(p => p.Campaigns)
                    .WithOne(p => p.AsteriskExtension)
                    .HasForeignKey(p => p.Extension)
                    .HasConstraintName("fk_campaign_extension");
            });

            modelBuilder.Entity<CampaignAbonentHistory>(entity =>
            {
                entity.ToTable("campaignabonenthistory");
                entity.HasKey(e => e.Id)
                    .HasName("pk_campaignabonenthistory");

                entity.HasOne(p => p.CampaignAbonent)
                    .WithMany(c => c.CampaignAbonentHistories)
                    .HasForeignKey(c => c.CampaignAbonentId)
                    .HasConstraintName("fk_campaignabonenthistory_campaignabonent");

                entity.HasOne(p => p.CallStatus)
                    .WithMany(c => c.CampaignAbonentHistories)
                    .HasForeignKey(c => c.Status)
                    .IsRequired(false)
                    .HasConstraintName("fk_campaignabonenthistory_callstatus");
                    
            });

            modelBuilder.Entity<CallStatus>(entity =>
            {
                entity.ToTable("callstatus");
                entity.HasKey(e => e.Id)
                    .HasName("pk_callstatus");
            });
        }
    }
}
