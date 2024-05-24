using CoinTrading.Api;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CoinTrading.Pages
{
    public class LatestPriceModel : PageModel
    {
        SystemDbContext DB;
        public LatestPriceModel()
        {
            DB = new SystemDbContext();
        }

        public JsonResult OnGet()
        {
            var lastPrice = DB.Prices.OrderByDescending(price => price.timestamp).FirstOrDefault();

            return new JsonResult(new { price = lastPrice });
        }
    }
}
