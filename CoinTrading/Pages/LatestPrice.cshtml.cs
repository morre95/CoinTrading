using CoinTrading.Api;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CoinTrading.Pages
{
    public class LatestPriceModel : PageModel
    {
        UserContext DB;
        public LatestPriceModel()
        {
            DB = new UserContext();
        }

        public JsonResult OnGet()
        {
            return new JsonResult(new { price = DB.Prices.FirstOrDefault() });
        }
    }
}
