using CoinTrading.Api;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CoinTrading.Pages
{
    public class LoginModel : PageModel
    {
        UserContext DB;
        public LoginModel()
        {
            DB = new UserContext();
        }

        public void OnGet()
        {
            var username = HttpContext.Session.GetString("Username");
            Console.WriteLine($"Username: {username}");
        }

        public IActionResult OnPost()
        {
            var name = Request.Form["Name"];
            var email = Request.Form["Email"];

            return RedirectToPage("./Index");
        }
    }
}
