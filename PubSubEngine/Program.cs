﻿using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Principal;
using Contracts;
using System.ServiceModel.Security;
using System.Security.Cryptography.X509Certificates;
using System.IdentityModel.Tokens;
using Manager;

namespace PubSubEngine
{
    public class Program
    {
        static void Main(string[] args)
        {
            /// srvCertCN.SubjectName should be set to the service's username. .NET WindowsIdentity class provides information about Windows user running the given process
			string srvCertCN = Formatter.ParseName(WindowsIdentity.GetCurrent().Name);
            Console.WriteLine(srvCertCN);

            NetTcpBinding binding = new NetTcpBinding();
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Certificate;

            string address = "net.tcp://localhost:4001/PubSubService";
            ServiceHost host = new ServiceHost(typeof(PubSubService));
            host.AddServiceEndpoint(typeof(IWCFContract), binding, address);
            //Console.ReadLine();

            ///Custom validation mode enables creation of a custom validator - CustomCertificateValidator
            host.Credentials.ClientCertificate.Authentication.CertificateValidationMode = X509CertificateValidationMode.Custom;
            host.Credentials.ClientCertificate.Authentication.CustomCertificateValidator = new ServiceCertValidator();

            ///If CA doesn't have a CRL associated, WCF blocks every client because it cannot be validated
            host.Credentials.ClientCertificate.Authentication.RevocationMode = X509RevocationMode.NoCheck;
           

            ///Set appropriate service's certificate on the host. Use CertManager class to obtain the certificate based on the "srvCertCN"
            host.Credentials.ServiceCertificate.Certificate = CertManager.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, srvCertCN);
            Console.WriteLine(CertManager.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, srvCertCN));

            
            //host.Open();
            //Console.ReadLine();

            try
            {
                //Console.WriteLine("radi");
                //Console.ReadLine();
                host.Open();
                Console.WriteLine("+++++++++ PUBLISH-SUBSCRIBE SERVIS JE USPESNO POKRENUT +++++++++");
                Console.WriteLine("Pitisnite [ENTER] za prekid.");
                Console.WriteLine("Hello from Sara!");
                Console.WriteLine("Hello from Sanja!");
                Console.ReadKey();
            }
            catch (Exception e)
            {
                Console.WriteLine("[ERROR] {0}", e.Message);
                Console.WriteLine("[StackTrace] {0}", e.StackTrace);
                Console.ReadLine();
            }
        }
    }
}
