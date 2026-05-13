using Microsoft.EntityFrameworkCore;
using TradingJournal.Models.Entity;
namespace TradingJournal.Context
{
    public class DBContext : DbContext
    {
        public DBContext(DbContextOptions<DBContext> options) : base(options) { }
        public DbSet <TradesEntity> Trades { get; set; }
        public DbSet <UserEntity> Users { get; set; }
        public DbSet <BrokerEntity> Brokers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TradesEntity>()
                .HasOne(t => t.User)
                .WithMany(u => u.Trades)
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<BrokerEntity>()
                .HasOne(b => b.User)
                .WithMany(u => u.Brokers)
                .HasForeignKey(b => b.UserID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<BrokerEntity>()
                .HasIndex(b => new { b.UserID, b.BrokerName })
                .IsUnique();
        }
    }
}
