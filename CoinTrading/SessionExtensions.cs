using CoinTrading.Api;
using Microsoft.AspNetCore.Http;
using System.Globalization;

namespace CoinTrading
{
    public static class SessionExtensions
    {
        public static bool GetBool(this ISession session, string key)
        {
            var data = session.Get(key);
            if (data == null)
            {
                return false;
            }
            return BitConverter.ToBoolean(data, 0);
        }

        public static void SetBool(this ISession session, string key, bool value)
        {
            session.Set(key, BitConverter.GetBytes(value));
        }

        public static void SetBalance(this ISession session, double balance)
        {
            session.SetString("Balance", balance.ToString());
        }

        public static double GetBalance(this ISession session)
        {
            bool isDouble = double.TryParse((session.GetString("Balance") ?? "").Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out double balnace);

            return isDouble ? balnace : 0.0;
        }

        public static bool IsLogedin(this ISession session) => session.GetBool("logedin") == true;
        public static string? GetUsername(this ISession session) => session.GetString("Username");
        public static string? GetEmail(this ISession session) => session.GetString("Email");

        public static void LoginMe(this ISession session, Users user)
        {
            session.SetString("Username", user.Username ?? "no Username");
            session.SetString("Email", user.Email ?? "no Email");
            session.SetBalance(user.Balance);
            session.SetBool("logedin", true);
        }

        public static void LogoutMe(this ISession session)
        {
            SystemDbContext db = new();
            Users? user = db.Users.Where(u => u.Username == session.GetUsername()).ToList().FirstOrDefault();

            if (user != null)
            {
                user.Token = "";
                db.SaveChanges();
            }

            session.Clear();
        }
    }
}
