using Microsoft.EntityFrameworkCore;

namespace CoinTrading.Api
{
    public class SystemDbContext : DbContext
    {
        public DbSet<Users> Users { get; set; }
        public DbSet<Prices> Prices { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(@"Data Source=Data\data.db;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Users>().ToTable("Users");
        }
 
    }

    public class Users
    {
        public int Id { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? Email { get; set; }
        public string? Token { get; set; }
        public double Balance { get; set; }
    }

    public class Prices
    {
        public int Id { get; set; }
        public string? symbol { get; set; }
        public double open_price { get; set; }
        public double close_price { get; set; }
        public double high_price { get; set; }
        public double low_price { get; set; }
        public string? timestamp { get; set; }
    }
}
