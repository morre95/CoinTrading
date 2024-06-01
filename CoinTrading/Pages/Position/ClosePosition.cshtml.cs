using CoinTrading.Api;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Diagnostics;
using System.Reflection.Metadata;

namespace CoinTrading.Pages.Position
{
    public class ClosePositionModel : PageModel
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
                    Debug.WriteLine($"Close position... amount: {amount}, price: {price}");
                    Positions? pos = db.Positions.FirstOrDefault(p => p.Userid == user.Id && p.is_closed == 0);
                    if (pos != null)
                    {
                        //double balance = HttpContext.Session.GetBalance();

                        // TODO: räkna ut och returnera något till frontend att det gick bra

                        /*double btcValue = amount / price;
                        double averageEntry = amount / btcValue;
                        double pAndL = amount * ((1 / averageEntry) - (1 / price));*/


                        var orders = db.Orders.Where(x => x.Positionid == pos.Id).ToList();

                        // TBD: Kanske något sånt här???
                        double pAndL = 0;
                        double totalOrderValue = 0;
                        orders.ForEach(o =>
                        {
                            double btcValue = o.Amount / o.OpenPrice;
                            double averageEntry = o.Amount / btcValue;
                            pAndL += o.Amount * ((1 / averageEntry) - (1 / price));

                            totalOrderValue += o.Amount;
                        });

                        orders.ForEach(o => o.ClosePrice = price);
                        db.SaveChanges();
                        pos.IsClosed = true;
                        db.SaveChanges();

                        user.Balance += pAndL * price + totalOrderValue;
                        db.SaveChanges();

                        HttpContext.Session.SetBalance(user.Balance);

                        CoinPairs[]? coinBalance = HttpContext.Session.GetCoinBalance();
                        coinBalance[0].Value = 0;
                        HttpContext.Session.SetCoinBalance(coinBalance);

                        return new JsonResult(new { success = $"Your order is closed", pAndL });

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
            return new JsonResult(new { error = $"Your amount is: {Request.Query["amount"]}, and price is: {Request.Query["price"]}, leverage is: {Request.Query["leverage"]}" });
        }
    }
}
