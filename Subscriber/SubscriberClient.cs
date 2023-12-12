using Common;
using Manager;
using PubSubEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Subscriber
{
    public class SubscriberClient : ChannelFactory<IEngine>, IDisposable

    {
        IEngine factory;

        public SubscriberClient(NetTcpBinding binding, string address) : base(binding, address) 
        {
            factory = this.CreateChannel();
        }


        public SubscriberClient(NetTcpBinding binding, EndpointAddress address)
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

         public void Dispose()
         {
             if (factory != null)
             {
                 factory = null;
             }

             this.Close();
         }

        public void Subscribe(string alarmTypes, string clientAddress)
        {
            try
            {
                factory.Subscribe(alarmTypes, clientAddress); //greska
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}", e);
                Console.ReadLine();
            }
        }

        public void Unsubscribe(string clientAddress)
        {
            try
            {
                factory.Unsubscribe(clientAddress);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: {0}", e);
                Console.ReadLine();
            }
        }
    }
}
