using CoinTrading.Api;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Text.Json;

namespace CoinTrading.Pages.Position
{

    public class OpenPositionModel : PageModel
    {
        public IActionResult OnGet()
        {
            Debug.WriteLine("Request.Query[\"amount\"]: " + Request.Query["amount"]);
            Debug.WriteLine("Request.Query[\"price\"]: " + Request.Query["price"]);
            Debug.WriteLine("double.TryParse(Request.Query[\"amount\"]...): " + double.TryParse(Request.Query["amount"].ToString().Replace('.', ','), out double amount2));
            Debug.WriteLine("double.TryParse(Request.Query[\"price\"]...): " + double.TryParse(Request.Query["price"].ToString().Replace('.', ','), out double price2));
            Debug.WriteLine("amount2: " + amount2);
            Debug.WriteLine("price2: " + price2);

            if (double.TryParse(Request.Query["amount"].ToString().Replace('.', ','), out double amount) &&
                double.TryParse(Request.Query["price"].ToString().Replace('.', ','), out double price) &&
                int.TryParse(Request.Query["leverage"], out int leverage))
            {
                string? side = Request.Query["side"];
                string? type = Request.Query["type"];

                double balance = HttpContext.Session.GetBalance();
                CoinPairs[]? coinBalance = HttpContext.Session.GetCoinBalance();

                PositionCalculator posCalc = new();
                if (side == "buy")
                {
                    posCalc.Buy(amount, price);
                }
                else if (side == "sell")
                {
                    posCalc.Sell(amount, price);
                }

                Debug.WriteLine("Pos value: " + posCalc.GetBTCValue());

                Debug.WriteLine($"Balance: {balance}");
                Debug.WriteLine($"if(pos.GetTotalValue() <= balance): {posCalc.GetBTCValue() <= balance}");
                if (amount <= balance)
                {
                    balance -= amount;
                    
                    HttpContext.Session.SetBalance(balance);

                    if (coinBalance == null)
                    {
                        coinBalance = new CoinPairs[1];
                        coinBalance[0] = new CoinPairs { Pair = CoinPairs.AvailablePair.btcusdt, Value = posCalc.GetBTCValue() };
                    }
                    else
                    {
                        coinBalance[0].Value += posCalc.GetBTCValue();
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
                    return new JsonResult(new { error = $"Balance not sufficient. Your balance: {balance}, Order value: {posCalc.GetBTCValue()}. Or you are not loged in" });
                }

                object jsonData = new
                {
                    orderValue = posCalc.GetBTCValue(),
                    pAndL = posCalc.GetFinalPAndL(),
                    averageEntry = posCalc.GetAvgEntryPrice(),
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

    public class PositionCalculator
    {
        private double Amount { get; set; }
        private double TotalBTCValue { get; set; }
        private double FinalPAndL { get; set; }

        public double GetAvgEntryPrice() => Amount / TotalBTCValue;
        public double GetBTCValue() => TotalBTCValue;
        public double GetQuantity() => Amount;
        public double GetPAndL(double price) => GetQuantity() * ((1 / GetAvgEntryPrice()) - (1 / price));

        public void Buy(double amount, double price)
        {
            Amount += amount;
            TotalBTCValue += amount / price;
        }

        public bool Sell(double amount, double price)
        {
            if (amount <= 0 || Amount - amount < 0)
            {
                return false;
            }

            Amount -= amount;
            TotalBTCValue -= amount / price;
            return true;
        }

        public bool ClosePosition(double price)
        {
            if (Amount > 0)
            {
                FinalPAndL = GetPAndL(price);
            }
            return Sell(Amount, price);
        }

        public double GetFinalPAndL()
        {
            return FinalPAndL;
        }
    }
}
