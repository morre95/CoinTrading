using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CoinTrading.Pages.Position
{
    public class ClosePositionModel : PageModel
    {
        public IActionResult OnGet()
        {
            return new JsonResult(new { error = $"Close position is not implemented yet" });
        }
    }
}
