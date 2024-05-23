using CoinTrading.Api;
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

            HttpContext.Session.SetString("Username", "true");
        }
    }

    
}
