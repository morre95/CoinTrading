using CoinTrading.Api;
using Microsoft.AspNetCore.Http;
using System.Globalization;
using System.Text.Json;

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

        /* public static void SetObject<T>(this ISession session, string key, T value)
         {
             session.SetString(key, JsonSerializer.Serialize(value));
         }

         public static T? GetObject<T>(this ISession session, string key)
         {
             var value = session.GetString(key);
             return value == null ? default : JsonSerializer.Deserialize<T>(value);
         }*/

        public static void SetBalance(this ISession session, double balance)
        {
            session.SetString("Balance", balance.ToString());
        }

        public static double GetBalance(this ISession session)
        {
            bool isDouble = double.TryParse((session.GetString("Balance") ?? "").Replace(',', '.'), NumberStyles.Any, CultureInfo.InvariantCulture, out double balnace);

            return isDouble ? balnace : 0.0;
        }

        public static void SetCoinBalance(this ISession session, params CoinPairs[] obj)
        {
            CoinPairs[]? old = session.GetCoinBalance();

            if (old == null)
            {
                session.SetString("Balance-coins", JsonSerializer.Serialize(obj));
            }
            else 
            {
                session.SetString("Balance-coins", JsonSerializer.Serialize(old.Concat(obj).ToArray()));
            }
        }

        public static CoinPairs[]? GetCoinBalance(this ISession session)
        {
            var value = session.GetString("Balance-coins");
            return value == null ? default : JsonSerializer.Deserialize<CoinPairs[]>(value);
        }

        public static void SetUserId(this ISession session, int id)
        {
            session.SetString("userId", id.ToString());
        }

        public static int GetUserId(this ISession session)
        {
            bool isId = int.TryParse(session.GetString("userId") ?? "", out int id);
            return isId ? id : -1;
        }

        public static void SetRedirect(this ISession session, string redirectTo)
        {
            session.SetString("redirectTo", redirectTo);
        }

        public static string? GetRedirect(this ISession session) => session.GetString("redirectTo");

        public static bool IsLogedin(this ISession session) => session.GetBool("logedin") == true;
        public static string? GetUsername(this ISession session) => session.GetString("Username");
        public static string? GetEmail(this ISession session) => session.GetString("Email");


        public static void LoginMe(this ISession session, Users user)
        {
            session.SetString("Username", user.Username ?? "no Username");
            session.SetString("Email", user.Email ?? "no Email");
            session.SetBalance(user.Balance);
            session.SetUserId(user.Id);
            session.SetBool("logedin", true);

            if (user.CoinBlances != null) 
            {
                session.SetCoinBalance(user.CoinBlances);
            }
            
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
