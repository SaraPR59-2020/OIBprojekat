using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AES
{
    public class Decryption
    {
        public static string DecryptString(string s, string SecretKey)
        {
            Console.WriteLine("0");
            string decryptedString = null;

            byte[] encryptedBytes = Encoding.UTF8.GetBytes(s);
            byte[] decryptedBytes = null;
            Console.WriteLine("1");

            AesCryptoServiceProvider aesCryptoProvider = new AesCryptoServiceProvider
            {
      
                Key = Encoding.UTF8.GetBytes(SecretKey),
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            };
            Console.WriteLine("2");

            ICryptoTransform aesDecryptTransform = aesCryptoProvider.CreateDecryptor();
            Console.WriteLine("3");
            using (MemoryStream memoryStream = new MemoryStream(encryptedBytes))
            {
                using (CryptoStream cryptoStream = new CryptoStream(memoryStream, aesDecryptTransform, CryptoStreamMode.Read))
                {
                      decryptedBytes = new byte[encryptedBytes.Length];
                      Console.WriteLine(decryptedBytes.Length);
                      cryptoStream.Read(decryptedBytes, 0, decryptedBytes.Length);
                      Console.WriteLine("read"); //provera
                      decryptedString = BitConverter.ToString(decryptedBytes);
                      Console.WriteLine("4"); //provera
                    
                }


            }

            return decryptedString;
        }

    }
}
