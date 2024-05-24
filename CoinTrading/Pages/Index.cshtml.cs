using CoinTrading.Api;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace CoinTrading.Pages
{
    public class IndexModel : PageModel
    {
        public string? Message { get; set; }
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public void OnGet(string? text)
        {
            Message = text;

            string? username = HttpContext.Session.GetString("Username");

            if (username != null) ViewData["Username"] = HttpContext.Session.GetString("Username");
        }
    }

    
}
