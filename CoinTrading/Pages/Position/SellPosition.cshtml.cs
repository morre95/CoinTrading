using CoinTrading.Api;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CoinTrading.Pages.Position
{
    public class SellPositionModel : PageModel
    {
        public IActionResult OnGet()
        {
            if (double.TryParse(Request.Query["amount"].ToString().Replace('.', ','), out double amount) &&
                double.TryParse(Request.Query["price"].ToString().Replace('.', ','), out double price) &&
                int.TryParse(Request.Query["leverage"], out int leverage))
            {
                SystemDbContext db = new();

                Users? user = db.Users.FirstOrDefault(u => u.Id == HttpContext.Session.GetUserId());

                if (user != null)
                {
                    Positions? pos = db.Positions.FirstOrDefault(p => p.Userid == user.Id && p.is_closed == 0);
                    if (pos != null)
                    {
                        double balance = HttpContext.Session.GetBalance();
                        CoinPairs[]? coinBalance = HttpContext.Session.GetCoinBalance();

                        // TODO: räkna ut och returnera något till frontend att det gick bra

                        /*double btcValue = amount / price;
                        double averageEntry = amount / btcValue;
                        double pAndL = amount * ((1 / averageEntry) - (1 / price));

                        pos.IsClosed = true;
                        db.SaveChanges();*/
                    }
                    else
                    {
                        return new JsonResult(new { error = $"You do not have an open position" });
                    }
                }
                else
                {
                    return new JsonResult(new { error = $"You are not logedin" });
                }

            }
            return new JsonResult(new { error = $"Sell orders is not implemented yet" });
        }
    }
}
