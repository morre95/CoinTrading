using CoinTrading.Api;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;
using System.Collections.Specialized;
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

        private string GetRedirectPage()
        {
            string? redirectPage = HttpContext.Session.GetRedirect();
            if (string.IsNullOrEmpty(redirectPage)) redirectPage = "./Index";

            HttpContext.Session.SetRedirect("");
            return redirectPage;
        }

        public IActionResult OnGet(string? text)
        {
            if (HttpContext.Session.IsLogedin() || CheckCookieLoggedin()) 
            {
                return RedirectToPage(GetRedirectPage(), new { text = $"Wellcome {HttpContext.Session.GetUsername()}" });
            }

            Message = text;

            string? username = HttpContext.Session.GetUsername();
            double balance = HttpContext.Session.GetBalance();

            if (username != null)
            {
                ViewData["Username"] = username;
                ViewData["Balance"] = balance;
            }

            return Page();
        }

        public IActionResult OnPostAsync()
        {
            var username = Request.Form["username"];
            var password = Request.Form["password"];
            var rememberMe = Request.Form["rememberMe"];

            IQueryable<Users> users = from u in DB.Users
                           where u.Username == username.ToString() && u.Password == Helper.GetPasswordHash(password.ToString())
                           select u;

            if (users.Count() == 1)
            {
                Users user = users.First();

                if (rememberMe == "on")
                {
                    user.Token = SetCookies(user.Username);
                    DB.SaveChanges();
                }

                HttpContext.Session.LoginMe(user);
                return RedirectToPage(GetRedirectPage(), new { text = $"Wellcome {user.Username}!" });
            }

            return RedirectToPage("./Login", new { text = "Wrong Username or Password" });
        }

        public string SetCookies(string username)
        {
            string token = Guid.NewGuid().ToString();
            HttpContext.Response.Cookies.Append("token", token);
            HttpContext.Response.Cookies.Append("username", username);
            HttpContext.Response.Cookies.Append("expires", DateTimeOffset.Now.AddDays(30).ToString());

            return token;
        }

        public bool CheckCookieLoggedin()
        {
            if (HttpContext.Session.IsLogedin()) return true;

            string? expires = HttpContext.Request.Cookies["expires"];

            if (expires != null && DateTimeOffset.TryParse(expires, out DateTimeOffset exDate) && exDate >= DateTimeOffset.Now)
            {
                string? token = HttpContext.Request.Cookies["token"];
                string? username = HttpContext.Request.Cookies["username"];
                if (token != null && username != null)
                {
                    Users? user = DB.Users.FirstOrDefault(x => x.Token == token && x.Username == username);
                    if (user != null) 
                    { 
                        HttpContext.Session.LoginMe(user);
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
