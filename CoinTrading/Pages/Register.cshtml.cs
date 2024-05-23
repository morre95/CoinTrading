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
            // TODO: fixa en check i javascript om lösenorden är samma och om det finns något mer som bör kollas
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
