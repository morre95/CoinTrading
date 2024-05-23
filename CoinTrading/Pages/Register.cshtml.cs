using CoinTrading.Api;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CoinTrading.Pages
{
    public class RegisterModel : PageModel
    {

        UserContext DB;
        public RegisterModel()
        {
            DB = new UserContext();
        }

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            // TODO: fixa en check i javascript om l�senorden �r samma och om det finns n�got mer som b�r kollas
            var username = Request.Form["username"];
            var email = Request.Form["email"];
            var password = Request.Form["password"];
            var passwordAgain = Request.Form["passwordAgain"];

            if (password != passwordAgain) return Page();

            Users user = new();

            user.Username = username;
            user.Email = email;
            user.Password = Helper.GetPasswordHash(password);

            DB.Add(user);
            DB.SaveChanges();

            return RedirectToPage("./Index");
        }

        
    }
}
