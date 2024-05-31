using CoinTrading.Api;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CoinTrading.Pages
{
    public class IndexModel : PageModel
    {
        public string? Message { get; set; }
        private readonly ILogger<IndexModel> _logger;
        private readonly SystemDbContext _db;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public void OnGet(string? text)
        {
            Message = text;

            string? username = HttpContext.Session.GetUsername();
            double balance = HttpContext.Session.GetBalance();

            if (username != null)
            {
                ViewData["Username"] = username;
                ViewData["Balance"] = balance;
            }
        }

    }


}
