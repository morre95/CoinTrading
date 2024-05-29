using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CoinTrading.Pages
{
    public class GameModel : PageModel
    {
        public bool IsLoegin { get; set; } = false;
        public void OnGet()
        {
            string? username = HttpContext.Session.GetUsername();
            double balance = HttpContext.Session.GetBalance();

            IsLoegin = HttpContext.Session.IsLogedin();

            HttpContext.Session.SetRedirect("Game");

            if (username != null)
            {
                ViewData["Username"] = username;
                ViewData["Balance"] = balance;
            }
        }

        
    }

    
}
