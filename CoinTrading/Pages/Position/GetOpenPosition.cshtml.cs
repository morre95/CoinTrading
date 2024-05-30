using CoinTrading.Api;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace CoinTrading.Pages.Position
{
    public class GetOpenPositionModel : PageModel
    {
        public class TestResult
        {
            public double average_price { get; set; }
            public double amount_sum { get; set; }
        }
        public IActionResult OnGet()
        {
            SystemDbContext db = new();

            Positions? pos = db.Positions.FirstOrDefault(p => p.Userid == HttpContext.Session.GetUserId() && p.is_closed == 0);

            if (pos != null)
            {
                //IQueryable<Orders> orders = db.Orders.FromSql($"SELECT *, AVG(open_price) AS avg_price, SUM(amount) AS amount_sum FROM orders WHERE positionid = {pos.Id}");
                var order = db.Orders.GroupBy(x => true).Select(o => new { avgPrice = o.Average(x => x.OpenPrice), sumAmount = o.Sum(x => x.Amount) }).FirstOrDefault();

                
                if (order != null) return new JsonResult(order);
                else return new JsonResult(new { error = "Cant find orders" });
            }

            return new JsonResult(new { error = "Something whent wrong" });
        }
    }
}
