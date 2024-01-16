using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class SecretKey
    {
     
        public static string GenerateKey()
        {
            string aesKey;
            using (Aes aesAlgorithm = Aes.Create())
            {
                aesAlgorithm.KeySize = 128;
                aesAlgorithm.GenerateKey();
                aesKey = Convert.ToBase64String(aesAlgorithm.Key);
            }

            return aesKey;
        }

        public static string GetKey(string outFile)
        {
            string fullPath =  outFile;
            string key;
            if (!File.Exists(fullPath))
            {
                key = GenerateKey();
                StoreKey(key, outFile);
            }
            else
            {
                key = LoadKey(fullPath);
            }

            return key;
        }

        #region STORING/LOADING
        public static void StoreKey(string secretKey, string outFile)
        {

            FileStream fOutput = new FileStream( outFile, FileMode.OpenOrCreate, FileAccess.Write);
            // Ključ se čuva u UTF-8 formatu jer je to podrazumevan format Windows OS fajl sistema
            byte[] buffer = Encoding.UTF8.GetBytes(secretKey);

            try
            {
                fOutput.Write(buffer, 0, buffer.Length);
            }
            catch (Exception e)
            {
                Console.WriteLine("SecretKeys.StoreKey:: ERROR {0}", e.Message);
            }
            finally
            {
                fOutput.Close();
            }
        }

        public static string LoadKey(string inFile)
        {
            // Ključ se čita iz UTF-8 formata jer je tako i sačuvan iz gore navedenog razloga
            FileStream fInput = new FileStream(inFile, FileMode.Open, FileAccess.Read);
            byte[] buffer = new byte[(int)fInput.Length];

            try
            {
                fInput.Read(buffer, 0, (int)fInput.Length);
            }
            catch (Exception e)
            {
                Console.WriteLine("SecretKeys.LoadKey:: ERROR {0}", e.Message);
            }
            finally
            {
                fInput.Close();
            }

            return Encoding.UTF8.GetString(buffer);
        }
        #endregion
    }
}
