using CoinTrading.Api;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CoinTrading.Pages
{
    public class PricesModel : PageModel
    {
        SystemDbContext DB;
        public PricesModel()
        {
            DB = new SystemDbContext();
        }

        public JsonResult OnGet()
        {
            return new JsonResult(new { prices = DB.Prices.ToList() });
        }
    }
}
