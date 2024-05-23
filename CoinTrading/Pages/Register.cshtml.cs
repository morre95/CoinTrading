using CoinTrading.Api;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Cryptography;

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
            user.Password = GetPasswordHash(password);

            DB.Add(user);
            DB.SaveChanges();

            return RedirectToPage("./Index");
        }

        private string GetPasswordHash(string password)
        {
            byte[] salt = RandomNumberGenerator.GetBytes(128 / 8); 

            return Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password!,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100000,
                numBytesRequested: 256 / 8));
        }
    }
}
