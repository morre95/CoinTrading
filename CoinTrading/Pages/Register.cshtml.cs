using CoinTrading.Api;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;
using System.Diagnostics;

namespace CoinTrading.Pages
{
    public class RegisterModel : PageModel
    {
        public string? Message { get; set; }
        UserContext DB;
        private readonly ILogger<IndexModel> _logger;
        public RegisterModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
            DB = new UserContext();
        }

        public void OnGet(string? text)
        {
            Message = text;

            string? username = HttpContext.Session.GetString("Username");

            if (username != null) ViewData["Username"] = HttpContext.Session.GetString("Username");
        }

        public IActionResult OnPost()
        {
            // TODO: fixa en check i javascript om lösenorden är samma och om det finns något mer som bör kollas
            var username = Request.Form["username"];
            var email = Request.Form["email"];
            var password = Request.Form["password"];
            var passwordAgain = Request.Form["passwordAgain"];

            /*int usernameCount = DB.Users.FromSql($"SELECT * FROM Users WHERE username = '{username}'").Count();
            Debug.WriteLine($"Användarnamn count: {usernameCount}");

            // TODO: Bör inte skickas till Error
            if (usernameCount > 0) 
            {
                Debug.WriteLine($"Användarnamn upptaget");
                return RedirectToPage("./Error");
            }

            int emailCount = DB.Users.FromSql($"SELECT * FROM Users WHERE email = {email}").ToList().Count;

            Debug.WriteLine($"Email count: {emailCount}");
            // TODO: Bör inte skickas till Error
            if (usernameCount > 0)
            {
                Debug.WriteLine($"Email upptaget");
                return RedirectToPage("./Error");
            }*/

            int usernameCount = 0, emailCount = 0;
            foreach (Users u in DB.Users)
            {
                if (u.Username == username)
                {
                    usernameCount++;
                }

                if (u.Email == email) 
                {
                    emailCount++;
                }
            }

            if (usernameCount > 0)
            {
                return RedirectToPage("./Register", new { text = "Username occupied" });
            }

            if (emailCount > 0)
            {
                return RedirectToPage("./Register", new { text = "Email occupied" });
            }

            if (password != passwordAgain || password == "")
            {
                return RedirectToPage("./Register", new { text = "Passwords do not match" });
            }

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
