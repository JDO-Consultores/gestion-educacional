using System;
using System.Security.Cryptography;
using System.Text;

namespace GestionColegios.Helpers
{
    public static class HashToken
    {
        public static string HashTokenPass(string token)
        {
            using (var sha = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(token);
                var hash = sha.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }
    }
}