using CoinTrading.Api;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CoinTrading.Pages
{
    public class LogoutModel : PageModel
    {
        public IActionResult OnGet()
        {
            string msg = $"Goodbye {HttpContext.Session.GetUsername()}!";
            
            HttpContext.Session.LogoutMe();

            HttpContext.Session.SetInfoAlert(msg);

            return RedirectToPage("./Index");
        }
    }
}
