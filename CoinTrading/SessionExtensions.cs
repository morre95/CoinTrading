using CoinTrading.Api;
using Microsoft.AspNetCore.Http;

namespace CoinTrading
{
    public static class SessionExtensions
    {
        public static bool? GetBool(this ISession session, string key)
        {
            var data = session.Get(key);
            if (data == null)
            {
                return null;
            }
            return BitConverter.ToBoolean(data, 0);
        }

        public static void SetBool(this ISession session, string key, bool value)
        {
            session.Set(key, BitConverter.GetBytes(value));
        }

        public static bool IsLogedin(this ISession session) => session.GetBool("logedin") == true;
        public static string? GetUsername(this ISession session) => session.GetString("Username");
        public static string? GetEmail(this ISession session) => session.GetString("Email");

        public static void LoginMe(this ISession session, Users user)
        {
            session.SetString("Username", user.Username);
            session.SetString("Email", user.Email);
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
