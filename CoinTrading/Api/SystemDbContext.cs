using CoinTrading.Pages.Position;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace CoinTrading.Api
{
    public class SystemDbContext : DbContext
    {
        public DbSet<Users> Users { get; set; }
        public DbSet<Prices> Prices { get; set; }
        public DbSet<Positions> Positions { get; set; }
        public DbSet<Orders> Orders { get; set; }

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

        [NotMapped]
        public CoinPairs[]? CoinBlances { 
            get => coin_balances != null ? JsonSerializer.Deserialize<CoinPairs[]>(coin_balances) ?? default : default; 
            set { 
                if (value != null) coin_balances = JsonSerializer.Serialize(value); 
            } 
        }

        public string? coin_balances { get; set; }
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

    public class Positions
    {
        public int Id { get; set; }
        public int Userid { get; set; }
        public string? Symbol { get; set; }
        public int Leverage { get; set; }

        [NotMapped]
        public bool IsClosed { get => is_closed == 1; set { is_closed = Convert.ToInt32(value); } }

        public int is_closed { get; set; }
        public DateTime Timestamp { get; set; }
    }

    public class Orders
    {
        public int Id { get; set; }
        public int Positionid { get; set; }

        [Column("open_price")]
        public double OpenPrice { get; set; }

        [Column("close_price")]
        public double ClosePrice { get; set; }

        public double Amount { get; set; }

        public string? Type { get; set; }
        public string? Side { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
