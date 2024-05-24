using CoinTrading.Api;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Diagnostics;

namespace CoinTrading.Pages.Validation
{
    public class UsernameModel : PageModel
    {
        public IActionResult OnGet()
        {
            SystemDbContext db = new();

            string? username = Request.Query["username"];

            if (username == null)
            {
                return new JsonResult(new { Error = "No username I know of" });
            }

            int count = db.Users.Where(user => user.Username == username).Count();

            return new JsonResult(new { Username = username, Count = count, Exists = count > 0 });
        }

    }
}
