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
            var username = Request.Form["username"];
            var password = Request.Form["password"];

            foreach (var u in DB.Users)
            {
                Console.WriteLine($"Username: {u.Username}, {u.Password}");
            }

            var user = DB.Users.FirstOrDefault(user => user.Username == username && user.Password == Helper.GetPasswordHash(password));

            if (user == null) 
            { 
                return RedirectToPage("./Error");
            }

            return RedirectToPage("./Index");
        }
    }
}
