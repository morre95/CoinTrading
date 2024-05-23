using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;
using System.Text;

namespace CoinTrading.Pages
{
    public class Helper
    {
        public static string GetPasswordHash(string password)
        {
            //byte[] salt = RandomNumberGenerator.GetBytes(128 / 8);
            
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
