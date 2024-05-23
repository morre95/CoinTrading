using CoinTrading.Api;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CoinTrading.Pages
{
    public class PricesModel : PageModel
    {
        UserContext DB;
        public PricesModel()
        {
            DB = new UserContext();
        }

        public JsonResult OnGet()
        {
            return new JsonResult(new { GustavsSuperVariabelWithPriceDataSoHeCanHaveASuperLongVariableToWorkWithBeacuseEveryProgramerLovesThat = DB.Prices.ToList() });
        }
    }
}
