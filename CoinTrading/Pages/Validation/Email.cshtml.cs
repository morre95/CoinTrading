using CoinTrading.Api;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CoinTrading.Pages.Validation
{
    public class EmailModel : PageModel
    {
        public IActionResult OnGet()
        {
            SystemDbContext db = new();

            string? email = Request.Query["email"];

            if (email == null || !Helper.IsEmail(email))
            {
                return new JsonResult(new { Error = "No email I know of" });
            }

            int count = db.Users.Where(user => user.Email == email).Count();

            return new JsonResult(new { Email = email, Count = count, Exists = count > 0 });
        }

        
    }
}
