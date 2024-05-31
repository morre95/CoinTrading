using CoinTrading.Api;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Diagnostics;

namespace CoinTrading.Pages.Position
{
    public class BuyPositionModel : PageModel
    {
        public IActionResult OnGet()
        {
            if (double.TryParse(Request.Query["amount"].ToString().Replace('.', ','), out double amount) &&
                double.TryParse(Request.Query["price"].ToString().Replace('.', ','), out double price) &&
                int.TryParse(Request.Query["leverage"], out int leverage))
            {
                string? side = Request.Query["side"];
                string? type = Request.Query["type"];
                double balance = HttpContext.Session.GetBalance();
                CoinPairs[]? coinBalance = HttpContext.Session.GetCoinBalance();

                double btcValue = amount / price;

                if (amount <= balance) 
                {
                    balance -= amount;

                    HttpContext.Session.SetBalance(balance);
                    if (coinBalance == null)
                    {
                        coinBalance = new CoinPairs[1];
                        coinBalance[0] = new CoinPairs { Pair = CoinPairs.AvailablePair.btcusdt, Value = btcValue };
                    }
                    else
                    {
                        coinBalance[0].Value += btcValue;
                        HttpContext.Session.SetCoinBalance(coinBalance);
                    }

                    SystemDbContext db = new();

                    //Users? user = db.Users.Where(u => u.Id == HttpContext.Session.GetUserId()).Select(u => u).FirstOrDefault();
                    Users? user = db.Users.FirstOrDefault(u => u.Id == HttpContext.Session.GetUserId());

                    if (user != null)
                    {
                        user.Balance = balance;
                        user.CoinBlances = coinBalance;
                        db.SaveChanges();

                        Positions? pos = db.Positions.FirstOrDefault(p => p.Userid == user.Id && p.is_closed == 0);
                        //var positions = db.Positions.Where(p => p.Userid == user.Id).ToList();
                        //Positions? pos = positions.FirstOrDefault(p => !p.IsClosed);

                        if (pos == null)
                        {
                            pos = new();
                            pos.Userid = user.Id;
                            pos.Symbol = "BTCUSDT";
                            pos.Leverage = leverage;
                            pos.IsClosed = false;
                            db.Add(pos);
                            db.SaveChanges();

                            Orders order = new();
                            order.Positionid = pos.Id;
                            order.OpenPrice = price;
                            order.Amount = amount;
                            order.Type = type;
                            order.Side = side;
                            db.Add(order);
                            db.SaveChanges();
                        }
                        else
                        {
                            Orders order = new();
                            order.Positionid = pos.Id;
                            order.OpenPrice = price;
                            order.Amount = amount;
                            order.Type = type;
                            order.Side = side;
                            db.Add(order);
                            db.SaveChanges();
                        }

                        Debug.WriteLine($"Balance: {balance}");
                        Debug.WriteLine($"coinBalance[0].Value: {coinBalance[0].Value}");
                    }
                    else
                    {
                        return new JsonResult(new { error = $"User don't exists. Id: '{HttpContext.Session.GetUserId()}'" });
                    }
                }
                else
                {
                    return new JsonResult(new { error = $"Balance not sufficient. Your balance: {balance}, Order value: {btcValue}. Or you are not loged in" });
                }
                double averageEntry = amount / btcValue;
                object jsonData = new
                {
                    orderValue = btcValue,
                    pAndL = amount * ((1 / averageEntry) - (1 / price)),
                    averageEntry,
                    price,
                    amount,
                    side,
                    type,
                    balance
                };

                return new JsonResult(jsonData);
            }

            return new JsonResult(new { error = "Something whent wrong" });
        }
    }
}
