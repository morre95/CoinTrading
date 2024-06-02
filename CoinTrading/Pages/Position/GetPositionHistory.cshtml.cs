using CoinTrading.Api;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CoinTrading.Pages.Position
{
    public class GetPositionHistoryModel : PageModel
    {
        public IActionResult OnGet()
        {
            SystemDbContext db = new();
            Users? user = db.Users.FirstOrDefault(u => u.Id == HttpContext.Session.GetUserId());

            if (user != null)
            {
                Positions? pos = db.Positions.FirstOrDefault(p => p.Userid == user.Id && p.is_closed == 0);
                if (pos != null)
                {
                    var order = db.Orders.Where(o => o.Positionid == pos.Id).Select(o => o);
                    if (order.Any())
                    {
                        return new JsonResult(order);
                    }
                }
                else
                {
                    return new JsonResult(new { error = "No open position to get history from" });
                }
            }
            return new JsonResult(new { error = "GetPositionHistory is not implemented yet" });
        }
    }
}
