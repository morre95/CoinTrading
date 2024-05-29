using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace CoinTrading.Pages
{
    public class ProfileModel : PageModel
    {
        private readonly ILogger<ProfileModel> _logger;

        [BindProperty]
        public string Username { get; set; }

        [BindProperty]
        public double Balance { get; set; }

        [BindProperty]
        [Required]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [BindProperty]
        [Required]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; }

        public ProfileModel(ILogger<ProfileModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
            Username = HttpContext.Session.GetUsername();
            Balance = HttpContext.Session.GetBalance();
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Logik f�r att uppdatera l�senordet
            _logger.LogInformation("Password for user {Username} is being updated.", Username);

            // Uppdatera l�senordet h�r

            return RedirectToPage("Profile");
        }
        //TODO: Add a method to change the password
        //TODO: Add a method to show user balance history
        //TODO: Add a method to show user trade history
    }
}
