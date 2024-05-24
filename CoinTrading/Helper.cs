using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;
using System.Text;

namespace CoinTrading
{
    public class Helper
    {
        public static string GetPasswordHash(string password)
        {
            // TODO: kolla strängen...
            byte[] salt = Encoding.ASCII.GetBytes("this is a random salt Amir should change. Erik refuses");

            return Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password!,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100000,
                numBytesRequested: 256 / 8));
        }
    }
}
