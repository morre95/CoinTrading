using CoinTrading.Api;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace CoinTrading.Pages
{
    public class ProfileModel : PageModel
    {
        private readonly ILogger<ProfileModel> _logger;


        public string? Username { get; set; }
        public double Balance { get; set; }

        public ProfileModel(ILogger<ProfileModel> logger)
        {
            _logger = logger;
        }

        public IActionResult OnGet()
        {
            if (!HttpContext.Session.IsLogedin())
            {
                return RedirectToPage("./Index");
            }

            Username = HttpContext.Session.GetUsername();
            Balance = HttpContext.Session.GetBalance();


            if (Username != null)
            {
                ViewData["Username"] = Username;
                ViewData["Balance"] = Balance;
            }

            return Page();
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var username = Request.Form["username"];
            var email = Request.Form["email"];
            var password = Request.Form["password"];
            var confirmPassword = Request.Form["confirmPassword"];

            int id = HttpContext.Session.GetUserId();

            SystemDbContext db = new SystemDbContext();
            Users? user = db.Users.FirstOrDefault(x => x.Id == id);

            if (user != null) 
            { 
                user.Password = Helper.GetPasswordHash(password.ToString());
                db.SaveChanges();
            }

            // Logik för att uppdatera lösenordet
            _logger.LogInformation($"Password for user {HttpContext.Session.GetUsername()} is being updated.");

            // Uppdatera lösenordet här

            return RedirectToPage("Profile");
        }
        //TODO: Add a method to change the password
        //TODO: Add a method to show user balance history
        //TODO: Add a method to show user trade history
    }
}
