using PubSubEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Subscriber
{
    public class Program
    {
        static void Main(string[] args)
        {
            string pubsubCN = "PubSubEngine";

            NetTcpBinding binding = new NetTcpBinding();
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Certificate;

            /// Use CertManager class to obtain the certificate based on the "srvCertCN" representing the expected service identity.
            X509Certificate2 srvCert = CertManager.GetCertificateFromStorage(StoreName.Root, StoreLocation.LocalMachine, pubsubCN);
            EndpointAddress address = new EndpointAddress(new Uri("net.tcp://localhost:4001/PubSubService"),
                                      new X509CertificateEndpointIdentity(srvCert));

            using (SubscriberClient proxy = new SubscriberClient(binding, address))
            {
                /// 1. Communication test
                proxy.TestCommunication();
                Console.WriteLine("TestCommunication() finished. Press <enter> to continue ...");

                Console.WriteLine("Uskoro cete imati uvid u dostupne teme ako ih ima...");
               //string rezultat = proxy.AddSubscriber(new PubSubEngine.Subscriber());
                //Console.WriteLine(rezultat);
                Console.ReadLine();
            }
        }
    }
}
