using CoinTrading.Api;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace CoinTrading.Pages
{
    public class LoginModel : PageModel
    {
        public string? Message { get; set; }
        SystemDbContext DB;
        private readonly ILogger<IndexModel> _logger;
        public LoginModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
            DB = new SystemDbContext();
        }

        public void OnGet(string? text)
        {
            Message = text;
            string? username = HttpContext.Session.GetUsername();

            if (username != null) ViewData["Username"] = username;
        }

        public IActionResult OnPost()
        {
            var username = Request.Form["username"];
            var password = Request.Form["password"];

            IQueryable<Users> users = from u in DB.Users
                           where u.Username == username.ToString() && u.Password == Helper.GetPasswordHash(password.ToString())
                           select u;

            if (users.Count() > 0)
            {
                Users user = users.First();
                HttpContext.Session.LoginMe(user);
                return RedirectToPage("./Index", new { text = $"Wellcome {user.Username}!" });
            }

            return RedirectToPage("./Login", new { text = "Wrong Username or Password" });
        }
    }
}
