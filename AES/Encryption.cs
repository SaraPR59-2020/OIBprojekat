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
            byte[] sBytes = Encoding.ASCII.GetBytes(s);
            byte[] encryptedBytes = null;
            string encryptedString = "";

            AesCryptoServiceProvider aesCryptoProvider = new AesCryptoServiceProvider
            {
                Key = Encoding.ASCII.GetBytes(SecretKey),
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
                    //encryptedString = Encoding.ASCII.GetString(encryptedBytes); //van
                }
            }
            encryptedString = Encoding.ASCII.GetString(encryptedBytes); //van
            return encryptedString;
        }
    }
}
