using Common;
using PubSubEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Manager;
using Formatter = Manager.Formatter;
using System.IO;
using System.Security.Cryptography;
using AES;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Collections;
using System.Security;

namespace Publisher
{
    public class PublisherClient : ChannelFactory<IEngine>, IDisposable

    {
        IEngine factory;

        public PublisherClient(NetTcpBinding binding, EndpointAddress address)
            : base(binding, address)
        {
            /// cltCertCN.SubjectName should be set to the client's username. .NET WindowsIdentity class provides information about Windows user running the given process
            string cltCertCN = Formatter.ParseName(WindowsIdentity.GetCurrent().Name);
            Console.WriteLine(cltCertCN);

            this.Credentials.ServiceCertificate.Authentication.CertificateValidationMode = System.ServiceModel.Security.X509CertificateValidationMode.Custom;
            this.Credentials.ServiceCertificate.Authentication.CustomCertificateValidator = new ClientsCertValidator();
            this.Credentials.ServiceCertificate.Authentication.RevocationMode = X509RevocationMode.NoCheck;

            /// Set appropriate client's certificate on the channel. Use CertManager class to obtain the certificate based on the "cltCertCN"
            this.Credentials.ClientCertificate.Certificate = CertManager.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, cltCertCN);

            factory = this.CreateChannel();
        }

        public void TestCommunication()
        {
            try
            {
                factory.TestCommunication();
            }
            catch (Exception e)
            {
                Console.WriteLine("[TestCommunication] ERROR = {0}", e.Message);
            }
        }

        public void SendDataToEngine(string alarm, byte[] sign)
        {
            try
            {
                string key = AES.SecretKey.GenerateKey();
                Console.WriteLine(alarm);
                Console.WriteLine(Directory.GetParent(System.IO.Directory.GetCurrentDirectory()).Parent.Parent.FullName);
                string startupPath = Path.Combine(Directory.GetParent(System.IO.Directory.GetCurrentDirectory()).Parent.Parent.FullName, "keyPubEng.txt");
                SecretKey.StoreKey(key, startupPath);
                //pravim bajtove, pozovem encr, vrati mi 
                //factory.SendDataToEngine(AES.Encryption.EncryptString(alarm, key), sign); //greska
                /*string encryptedString = AES.Encryption.EncryptString(alarm, key);
                byte[] encryptedBytes = Encoding.UTF8.GetBytes(encryptedString);
                byte[] combinedBytes = new byte[encryptedBytes.Length + sign.Length];
                factory.SendDataToEngine(alarm, combinedBytes);*/
                Console.WriteLine(key);
                //string enkriptovanAlarm = AES.Encryption.EncryptString(alarm, key);
                //Console.WriteLine("Enkriptovan alarm je: " + enkriptovanAlarm);
                string enkriptovanString = AES.Encryption.EncryptString(alarm, key);
                factory.SendDataToEngine(enkriptovanString, sign);

                //factory.SendDataToEngine(enkriptovanAlarm, sign); //2
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}", e);
            }
        }

    }
}
