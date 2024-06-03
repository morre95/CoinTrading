using CoinTrading.Api;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Diagnostics;
using System.Security.Cryptography;

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
                                double pAndL = 0;
                                posValue -= btcValue;
                                if (posValue == 0)
                                {
                                    pos.IsClosed = true;
                                    db.SaveChanges();

                                    var orders = db.Orders.Where(x => x.Positionid == pos.Id).ToList();

                                    
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

                                // TODO: N�gon slags loop h�r som g�r igenom tabellen och t�mmer ordrar tills amount variabeln �r p� noll
                                // Exempel...
                                pAndL = 0;
                                double totalOrderValue = 0;
                                foreach (var item in db.Orders.Where(x => x.Positionid == pos.Id))
                                {
                                    if (item.Amount < amount)
                                    {
                                        amount -= item.Amount;
                                        item.Amount = 0;
                                        item.ClosePrice = price;
                                        db.SaveChanges();

                                        // TBD: P&L b�r r�knas ut p� n�ge vis h�r...???
                                        btcValue = item.Amount / item.OpenPrice;
                                        double averageEntry = item.Amount / btcValue;
                                        pAndL += item.Amount * ((1 / averageEntry) - (1 / price));

                                        totalOrderValue += item.Amount;

                                        /*Debug.WriteLine(double.IsNaN(pAndL));
                                        Debug.WriteLine(pAndL > 0);
                                        Debug.WriteLine(totalOrderValue > 0);*/

                                    }
                                    else if (item.Amount >= amount)
                                    {
                                        // TBD: Ska detta sparas???
                                        //pos.IsClosed = true;
                                        //db.SaveChanges();

                                        CoinPairs[] coinBalance = [new()];
                                        coinBalance[0].Pair = CoinPairs.AvailablePair.btcusdt;
                                        coinBalance[0].Value = 0;
                                        HttpContext.Session.SetCoinBalance(coinBalance);

                                        user.Balance += pAndL * price + totalOrderValue;
                                        user.CoinBlances = coinBalance;
                                        db.SaveChanges();

                                        HttpContext.Session.SetBalance(user.Balance);

                                        // TBD: kanske partialClosePrice ska finnas med h�r eller P&L???
                                        item.Amount -= amount;
                                        db.SaveChanges();

                                        // TODO: r�kna ut p&l f�r delar av ordern
                                        return new JsonResult(new { success = $"Part of your order is closed", pAndL });
                                    }
                                }

                                return new JsonResult(new { success = $"Year order is still open.", pAndL });
                            }
                            else
                            {
                                Debug.WriteLine("Japp denna k�rs");
                                return RedirectToPage("/Close/Position", new { amount, price, leverage });
                                // TODO: Redirecta anv�ndaren till close pos h�r eftersom den f�rs�ker s�lja f�r mer
                                //return new JsonResult(new { error = $"Your open psoisition is not that big. \nTotal sum of your order: {posValue}\nThe sum of what you try to sell: {btcValue}" });
                            }
                        }
                        else
                        {
                            return new JsonResult(new { error = $"You do not have any orders filled yet" });
                        }
                        // TODO: r�kna ut och returnera n�got till frontend att det gick bra

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
