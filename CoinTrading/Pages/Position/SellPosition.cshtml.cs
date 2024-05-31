using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CoinTrading.Pages.Position
{
    public class SellPositionModel : PageModel
    {
        public IActionResult OnGet()
        {
            return new JsonResult(new { error = $"Sell orders is not implemented yet" });
        }
    }
}
