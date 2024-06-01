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

                        var order = db.Orders.Where(o => o.Positionid == pos.Id).GroupBy(x => true).Select(o => new { avgPrice = o.Average(x => x.OpenPrice), sumAmount = o.Sum(x => x.Amount) }).FirstOrDefault();

                        if (order != null) 
                        {
                            double btcValue = amount / price;
                            double posValue = order.sumAmount / order.avgPrice;

                            if (btcValue <= posValue)
                            {
                                posValue -= btcValue;
                                if (posValue == 0)
                                {
                                    pos.IsClosed = true;
                                    db.SaveChanges();

                                    var orders = db.Orders.Where(x => x.Positionid == pos.Id).ToList();

                                    double pAndL = 0;
                                    orders.ForEach(o =>
                                    {
                                        double btcValue = o.Amount / o.OpenPrice;
                                        double averageEntry = o.Amount / btcValue;
                                        pAndL += o.Amount * ((1 / averageEntry) - (1 / price));
                                    });

                                    orders.ForEach(o => o.ClosePrice = price);
                                    db.SaveChanges();

                                    return new JsonResult(new { success = $"Your order is closed", pAndL });
                                }

                                // TODO: Någon slags loop här som går igenom tabellen och tömmer ordrar tills amount variabeln är på noll
                                // Exempel...
                                foreach (var item in db.Orders.Where(x => x.Positionid == pos.Id))
                                {
                                    if (item.Amount < amount)
                                    {
                                        amount -= item.Amount;
                                        item.Amount = 0;
                                        item.ClosePrice = price;
                                        db.SaveChanges();

                                        // TBD: P&L bär räknas ut på någe vis här...???

                                    }
                                    else if (item.Amount >= amount)
                                    {
                                        // TBD: kanske partialClosePrice ska finnas med här eller P&L???
                                        item.Amount -= amount;
                                        db.SaveChanges();

                                        // TODO: räkna ut p&l här och returnera det
                                        return new JsonResult(new { success = $"Part of your order is closed" });
                                    }
                                }

                                // TBD: Borde alldrig komma hit???
                                return new JsonResult(new { success = $"Year order is still open." });
                            }
                            else
                            {
                                return new JsonResult(new { error = $"Your open psoisition is not that big. \nTotal sum of your order: {posValue}\nThe sum of what you try to sell: {btcValue}" });
                            }
                        }
                        else
                        {
                            return new JsonResult(new { error = $"You do not have any orders filled yet" });
                        }
                        // TODO: räkna ut och returnera något till frontend att det gick bra

                        /*double btcValue = amount / price;
                        double averageEntry = amount / btcValue;
                        double pAndL = amount * ((1 / averageEntry) - (1 / price));

                        pos.IsClosed = true;
                        db.SaveChanges();*/
                    }
                    else
                    {
                        return new JsonResult(new { error = $"You do not have any open position" });
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
