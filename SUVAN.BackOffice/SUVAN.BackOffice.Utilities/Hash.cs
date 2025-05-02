using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SUVAN.BackOffice.Utilities
{
    public static class Hash
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string HashTexto(this string input)
        {
            // Calcular el hash SHA-256 de la cadena de entrada
            byte[] hashedBytes = GeneraHash56(input);
            string hashedString = Convert.ToBase64String(hashedBytes);
            return hashedString;

        }

        private static byte[] GeneraHash56(string input)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));

                // Convertir los bytes hash a una cadena en formato base64
                return hashedBytes;
            }
        }

        public static string GetHashSHA256(this string input)
        {
            // Calcular el hash SHA-256 de la cadena de entrada
            byte[] hashedBytes = GeneraHash56(input);
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hashedBytes.Length; i++) sb.AppendFormat("{0:x2}", hashedBytes[i]);
            return sb.ToString();
        }

    }
}
