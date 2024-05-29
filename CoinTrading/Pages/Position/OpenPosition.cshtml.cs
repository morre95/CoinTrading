using CoinTrading.Api;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Diagnostics;

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

                Position pos = new();
                if (side == "buy")
                {
                    pos.Buy(amount, price);
                }
                else if (side == "sell")
                {
                    pos.Sell(amount, price);
                }

                Debug.WriteLine("Pos value: " + pos.GetTotalValue());

                Debug.WriteLine($"Balance: {balance}");
                Debug.WriteLine($"if(pos.GetTotalValue() <= balance): {pos.GetTotalValue() <= balance}");
                if (pos.GetTotalValue() <= balance)
                {
                    balance -= pos.GetTotalValue() * amount;
                    HttpContext.Session.SetBalance(balance);

                    SystemDbContext db = new();

                    Users? user = db.Users.Where(u => u.Id == HttpContext.Session.GetUserId()).Select(u => u).FirstOrDefault();

                    if (user != null)
                    {
                        user.Balance = balance;
                        db.SaveChanges();
                        Debug.WriteLine($"Balance: {balance}");
                    }
                    else
                    {
                        return new JsonResult(new { error = $"User don't exists. Id: '{HttpContext.Session.GetUserId()}'" });
                    }

                }
                else
                {
                    return new JsonResult(new { error = $"Balance not sufficient. Your balance: {balance}, Order value: {pos.GetTotalValue()}. Or you are not loged in" });
                }

                object jsonData = new
                {
                    orderValue = pos.GetTotalValue(),
                    pAndL = pos.GetFinalPAndL(),
                    price,
                    amount,
                    side,
                    type,
                };

                return new JsonResult(jsonData);
            }

            return new JsonResult(new { error = "Something whent wrong" });
        }
    }

    public class Order
    {
        public enum OrderType { limiit, market }
        public enum OrderSide { buy, sell }
        public double Price { get; set; }
        public double Amount { get; set; }
        public int Leverage { get; set; }
        public double OrderValue { get; set; }
    }

    public class Position
    {
        private double Quantity { get; set; }
        private double TotalValue { get; set; }
        private double FinalPAndL { get; set; }

        public double GetAvgEntryPrice() => Quantity / TotalValue;
        public double GetTotalValue() => TotalValue;
        public double GetQuantity() => Quantity;
        public double GetPAndL(double price) => GetQuantity() * ((1 / GetAvgEntryPrice()) - (1 / price));

        public void Buy(double quantity, double price)
        {
            Quantity += quantity;
            TotalValue += quantity / price;
        }

        public bool Sell(double quantity, double price)
        {
            if (quantity <= 0 || Quantity - quantity < 0)
            {
                return false;
            }

            Quantity -= quantity;
            TotalValue -= quantity / price;
            return true;
        }

        public bool ClosePosition(double price)
        {
            if (Quantity > 0)
            {
                FinalPAndL = GetPAndL(price);
            }
            return Sell(Quantity, price);
        }

        public double GetFinalPAndL()
        {
            return FinalPAndL;
        }
    }
}
