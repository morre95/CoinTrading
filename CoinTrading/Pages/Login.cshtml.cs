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
        UserContext DB;
        private readonly ILogger<IndexModel> _logger;
        public LoginModel(ILogger<IndexModel> logger)
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
            var username = Request.Form["username"];
            var password = Request.Form["password"];

            //var user = DB.Users.FirstOrDefault(user => user.Username == username && user.Password == Helper.GetPasswordHash(password));
            //var userTest = DB.Users.Select(user => user).Where(user => user.Username == username && user.Password == Helper.GetPasswordHash(password));
            var userTest = from u in DB.Users
                           where u.Username == username && u.Password == Helper.GetPasswordHash(password)
                           select u;
            Debug.WriteLine("Detta är Debug");
            Debug.WriteLine(userTest);
            foreach (var u in userTest)
            {
                Debug.WriteLine($"{u.Username}");
            }
            //var user = DB.Users.FromSql($"SELECT * FROM Users WHERE (username = '{username}' AND password = '{Helper.GetPasswordHash(password)}')").FirstOrDefault();
            //var user = DB.Users.FromSql($"SELECT * FROM Users WHERE password = {Helper.GetPasswordHash(password)}").FirstOrDefault();
            //var user = DB.Users.FromSql($"SELECT * FROM Users WHERE username = {username} AND password = {Helper.GetPasswordHash(password)}").First();

            Users? user = null;
            foreach (Users item in DB.Users) 
            {
                if (item.Username == username && item.Password == Helper.GetPasswordHash(password)) 
                {
                    user = item;
                    break;
                }
            }

            //Debug.WriteLine($"{username} {password}, '{Helper.GetPasswordHash(password)}'");
            if (user == null) 
            { 
                return RedirectToPage("./Login", new { text = "Wrong Username or Password" });
            }
            else
            {
                HttpContext.Session.SetString("Username", user.Username);
            }

            return RedirectToPage("./Index", new { text = $"Wellcome {user.Username}!" });
        }
    }
}
