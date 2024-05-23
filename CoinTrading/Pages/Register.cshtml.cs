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

            /*int usernameCount = DB.Users.FromSql($"SELECT * FROM Users WHERE username = '{username}'").Count();
            Debug.WriteLine($"Anv�ndarnamn count: {usernameCount}");

            // TODO: B�r inte skickas till Error
            if (usernameCount > 0) 
            {
                Debug.WriteLine($"Anv�ndarnamn upptaget");
                return RedirectToPage("./Error");
            }

            int emailCount = DB.Users.FromSql($"SELECT * FROM Users WHERE email = {email}").ToList().Count;

            Debug.WriteLine($"Email count: {emailCount}");
            // TODO: B�r inte skickas till Error
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

            // TODO: B�r inte skickas till Error
            if (usernameCount > 0)
            {
                Debug.WriteLine($"Anv�ndarnamn upptaget");
                return RedirectToPage("./Error");
            }

            // TODO: B�r inte skickas till Error
            if (usernameCount > 0)
            {
                Debug.WriteLine($"Email upptaget");
                return RedirectToPage("./Error");
            }

            if (password != passwordAgain)
            {
                Debug.WriteLine($"L�senorden st�mmer inte");
                return RedirectToPage("./Error");
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
