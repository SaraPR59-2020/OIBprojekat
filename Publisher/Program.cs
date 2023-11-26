using System;
using System.Collections.Generic;
using PubSubEngine;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography.X509Certificates;
using Common;

namespace Publisher
{
    public class Program
    {

        static void Main(string[] args)
        {
            /// Define the expected service certificate. It is required to establish cmmunication using certificates.
            string pubsubCN = "PubSubEngine";

            NetTcpBinding binding = new NetTcpBinding();
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Certificate;

            /// Use CertManager class to obtain the certificate based on the "srvCertCN" representing the expected service identity.
            X509Certificate2 srvCert = CertManager.GetCertificateFromStorage(StoreName.Root, StoreLocation.LocalMachine, pubsubCN);
            EndpointAddress address = new EndpointAddress(new Uri("net.tcp://localhost:4001/PubSubService"),
                                      new X509CertificateEndpointIdentity(srvCert));
            

            using (PublisherClient proxy = new PublisherClient(binding, address))
            {
                /// 1. Communication test
                proxy.TestCommunication();
                Console.WriteLine("TestCommunication() finished. Press <enter> to continue ...");

                Console.WriteLine("Unesite temu:");
                string topic = Console.ReadLine();
                string rezultat = proxy.AddPublisher(new Common.Publisher(new Topic(topic)));
                Console.WriteLine(rezultat);
                Console.ReadLine();

            }

        }
    }
}
