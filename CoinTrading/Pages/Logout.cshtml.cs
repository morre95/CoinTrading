using CoinTrading.Api;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CoinTrading.Pages
{
    public class LogoutModel : PageModel
    {
        public IActionResult OnGet()
        {
            HttpContext.Session.SetInfoAlert($"Goodbye {HttpContext.Session.GetUsername()}!");
            HttpContext.Session.LogoutMe();

            return RedirectToPage("./Index");
        }
    }
}
