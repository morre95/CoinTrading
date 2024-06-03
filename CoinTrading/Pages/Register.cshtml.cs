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
        SystemDbContext DB;
        private readonly ILogger<IndexModel> _logger;
        public RegisterModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
            DB = new SystemDbContext();
        }

        public IActionResult OnGet(string? text)
        {
            Message = text;

            if (HttpContext.Session.IsLogedin())
            {
                return RedirectToPage("./Index");
            }

            return Page();
        }

        public IActionResult OnPost()
        {
            var username = Request.Form["username"];
            var email = Request.Form["email"];
            var password = Request.Form["password"];
            var confirmPassword = Request.Form["confirmPassword"];


            /*IQueryable<Users> emailUsers = from u in DB.Users
                                      where u.Email == email.ToString()
                                      select u;

            Debug.WriteLine($"Emails: {emailUsers.Count()}");*/

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

            if (password != confirmPassword || password == "")
            {
                return RedirectToPage("./Register", new { text = "Passwords do not match" });
            }

            Users user = new();

            user.Username = username;
            user.Email = email;
            user.Password = Helper.GetPasswordHash(password.ToString());
            user.Balance = 10_000.0;

            DB.Add(user);
            DB.SaveChanges();

            HttpContext.Session.SetSuccessAlert($"Congrats {user.Username}, you successfully register a user account");

            return RedirectToPage("./Index");
        }



    }
}
