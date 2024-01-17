using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Principal;
using System.ServiceModel.Security;
using System.Security.Cryptography.X509Certificates;
using System.IdentityModel.Tokens;
using Manager;
using Common;
using System.ServiceModel.Description;
using System.Diagnostics;

namespace PubSubEngine
{
    public class Program
    {
        static void Main(string[] args)
        {
            //Debugger.Launch();
            string srvCertCN = "PubSubEngine";
            Console.WriteLine(srvCertCN);

            NetTcpBinding binding = new NetTcpBinding();
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Certificate;

            string address = "net.tcp://localhost:4001/PubSubService";
            ServiceHost host = new ServiceHost(typeof(PubSubService));
            host.AddServiceEndpoint(typeof(IEngine), binding, address);

            ///Custom validation mode enables creation of a custom validator - CustomCertificateValidator
            host.Credentials.ClientCertificate.Authentication.CertificateValidationMode = X509CertificateValidationMode.Custom;
            host.Credentials.ClientCertificate.Authentication.CustomCertificateValidator = new ServiceCertValidator();
            
            host.Credentials.ClientCertificate.Authentication.RevocationMode = X509RevocationMode.NoCheck;
            host.Credentials.ServiceCertificate.Certificate = CertManager.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, srvCertCN);

            //Audit audit = new Audit(); //napravi audit
            try
            {
                host.Open();
                Console.WriteLine("+++++++++ PUBLISH-SUBSCRIBE ENGINE +++++++++");
                Console.WriteLine("Press [ENTER] to end.");
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
