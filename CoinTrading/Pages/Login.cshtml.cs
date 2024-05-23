using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CoinTrading.Pages
{
    public class LoginModel : PageModel
    {
        public void OnGet()
        {
            var username = HttpContext.Session.GetString("Username");
            Console.WriteLine($"Username: {username}");
        }
    }
}
