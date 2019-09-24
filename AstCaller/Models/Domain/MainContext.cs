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

            modelBuilder.Entity<User>().ToTable("users");
            modelBuilder.Entity<Campaign>().ToTable("campaign");

            modelBuilder.Entity<CampaignAbonent>()
                .ToTable("campaignabonent")
                .Property(p => p.HasErrors).HasConversion<int>();
        }
    }
}
