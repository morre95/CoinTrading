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
        UserContext DB;
        public LoginModel()
        {
            DB = new UserContext();
        }

        public void OnGet()
        {
            var username = HttpContext.Session.GetString("Username");
            if (username == null)
            {
                Debug.WriteLine("Inte inloggad");
            }
            else 
            {
                Debug.WriteLine($"Username: {username} är inloggad");
            }
        }

        public IActionResult OnPost()
        {
            var username = Request.Form["username"];
            var password = Request.Form["password"];

            //var user = DB.Users.FirstOrDefault(user => user.Username == username && user.Password == Helper.GetPasswordHash(password));
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

            Debug.WriteLine($"{username} {password}, '{Helper.GetPasswordHash(password)}'");
            if (user == null) 
            { 
                return RedirectToPage("./Error");
            }
            else
            {
                HttpContext.Session.SetString("Username", user.Username);
            }

            return RedirectToPage("./Index");
        }
    }
}
