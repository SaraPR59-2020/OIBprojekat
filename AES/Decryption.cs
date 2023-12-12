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
        /*public static string DecryptString( byte[] chyperText, string secretKey)
        {
            string decryptedText = null;

            AesCryptoServiceProvider aesCryptoProvider = new AesCryptoServiceProvider
            {
                Key = ASCIIEncoding.ASCII.GetBytes(secretKey),
                Mode = CipherMode.ECB,
                Padding = PaddingMode.PKCS7
            };

            ICryptoTransform aesDecryptTransform = aesCryptoProvider.CreateDecryptor();

            using (MemoryStream memoryStream = new MemoryStream(chyperText.Skip(aesCryptoProvider.BlockSize / 8).ToArray()))
            {
                using (CryptoStream cryptoStream = new CryptoStream(memoryStream, aesDecryptTransform, CryptoStreamMode.Read))
                {
                    using (StreamReader reader = new StreamReader(cryptoStream, Encoding.UTF8))
                    {
                        decryptedText = reader.ReadToEnd();
                    }
                }
            }

            return decryptedText;
        }*/
         public static string DecryptString(byte[] chyperText, string secretKey)
         {
             string decryptedString = null;

             //byte[] encryptedBytes = Encoding.ASCII.GetBytes(secretKey);
             byte[] decryptedBytes = null;

             AesCryptoServiceProvider aesCryptoProvider = new AesCryptoServiceProvider
             {

                 Key = ASCIIEncoding.ASCII.GetBytes(secretKey),
                 Mode = CipherMode.ECB,
                 Padding = PaddingMode.PKCS7
             };

             ICryptoTransform aesDecryptTransform = aesCryptoProvider.CreateDecryptor();
             using (MemoryStream memoryStream = new MemoryStream(chyperText))
             {
                 using (CryptoStream cryptoStream = new CryptoStream(memoryStream, aesDecryptTransform, CryptoStreamMode.Read))
                 {
                       decryptedBytes = new byte[chyperText.Length];
                       cryptoStream.Read(decryptedBytes, 0, decryptedBytes.Length);
                       //decryptedString = Encoding.ASCII.GetString(decryptedBytes);

                 }
             }
             decryptedString = Encoding.ASCII.GetString(decryptedBytes); 
             return decryptedString;
         }

        

    }
}
