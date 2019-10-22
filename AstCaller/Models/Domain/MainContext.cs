using Microsoft.EntityFrameworkCore;

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
                    .HasConstraintName("fk_campaignabonent_campaign");
                entity.HasOne(p => p.Modifier)
                    .WithMany(u => u.CampaignAbonents)
                    .HasConstraintName("fk_campaignabonent_modifier");
            });
        }
    }
}
