using Microsoft.EntityFrameworkCore;
using TradingJournal.Models.Entity;
namespace TradingJournal.Context
{
    public class DBContext : DbContext
    {
        public DBContext(DbContextOptions<DBContext> options) : base(options) { }
        public DbSet <TradesEntity> Trades { get; set; }
        public DbSet <UserEntity> Users { get; set; }
    }
}
