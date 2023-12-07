using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AES
{
    public class Encryption
    {
        public static string EncryptString(string s, string SecretKey)
        {
            // byte[] sBytes = Convert.FromBase64String(s); //poslednja greska
            byte[] sBytes = Encoding.UTF8.GetBytes(s);
            byte[] encryptedBytes = null;
            string encryptedString = "";

            AesCryptoServiceProvider aesCryptoProvider = new AesCryptoServiceProvider
            {
                Key = Encoding.UTF8.GetBytes(SecretKey),
            Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            };

            ICryptoTransform aesEncryptTransform = aesCryptoProvider.CreateEncryptor();
            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (CryptoStream cryptoStream = new CryptoStream(memoryStream, aesEncryptTransform, CryptoStreamMode.Write))
                {
                    cryptoStream.Write(sBytes, 0, sBytes.Length);
                    encryptedBytes = memoryStream.ToArray();
                    encryptedString = Convert.ToBase64String(encryptedBytes);
                }
            }
            return encryptedString;
        }
    }
}
