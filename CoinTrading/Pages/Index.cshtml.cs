using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace CoinTrading.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
            UserContext user = new UserContext();

            foreach (var u in user.Users)
            {
                Debug.WriteLine($"{u.Username}");
                Debug.WriteLine($"{u.Email}");
                Debug.WriteLine($"{u.Password}");
            }
        }
    }

    public class Users
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
    }

    [Keyless]
    public class Prices
    {
        public string symbol { get; set; }
        public double open_price { get; set; }
        public double close_price { get; set; }
        public double high_price { get; set; }
        public double low_price { get; set; }
        public string timestamp { get; set; }
    }

    public class UserContext : DbContext
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
}
