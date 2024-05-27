using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CoinTrading.Pages
{
    public class GameModel : PageModel
    {
        public void OnGet()
        {
            string? username = HttpContext.Session.GetUsername();
            double balance = HttpContext.Session.GetBalance();

            if (username != null)
            {
                ViewData["Username"] = username;
                ViewData["Balance"] = balance;
            }
        }

        public IActionResult OnPost()
        {
            //var amount = Request.Form["amount"];
            //var price = Request.Form["price"];
            var leverage = Request.Form["leverage"];
            var side = Request.Form["leverage"];

            Position pos = new();
            if (double.TryParse(Request.Form["amount"], out double amount) && double.TryParse(Request.Form["price"], out double price))
            {
                if (side == "buy")
                {
                    pos.Buy(amount, price);
                }
                else if (side == "sell")
                {
                    pos.Sell(amount, price);
                }


                double balance = HttpContext.Session.GetBalance();

                if (pos.GetTotalValue() <= balance) 
                {
                    balance -= pos.GetTotalValue();
                    HttpContext.Session.SetBalance(balance);

                    // TODO: sapara i databas eller liknande...
                }

            }

            return new JsonResult( new { orderValue = pos.GetTotalValue() } );
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
