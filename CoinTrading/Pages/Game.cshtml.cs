using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CoinTrading.Pages
{
    public class GameModel : PageModel
    {
        public void OnGet()
        {
            string? username = HttpContext.Session.GetUsername();

            if (username != null) ViewData["Username"] = username;
        }
    }
}
