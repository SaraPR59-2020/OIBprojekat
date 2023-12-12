using System;
using System.Collections.Generic;
using PubSubEngine;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography.X509Certificates;
using Common;
using System.Security.Principal;
using Manager;
using System.Threading;

namespace Publisher
{
    public class Program
    {

        static void Main(string[] args)

        { 

            string signCertCN = Formatter.ParseName(WindowsIdentity.GetCurrent().Name) + "_sign"; //kreiram digitalni sertf publisher_sign
            /// Define the expected service certificate. It is required to establish cmmunication using certificates.
            string pubsubCN = "PubSubEngine";

            NetTcpBinding binding = new NetTcpBinding();
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Certificate;

            /// Use CertManager class to obtain the certificate based on the "srvCertCN" representing the expected service identity.
            X509Certificate2 srvCert = CertManager.GetCertificateFromStorage(StoreName.Root, StoreLocation.LocalMachine, pubsubCN);
            EndpointAddress address = new EndpointAddress(new Uri("net.tcp://localhost:4001/PubSubService"),
                                      new X509CertificateEndpointIdentity(srvCert));
            


            using(PublisherClient proxy = new PublisherClient(binding, address))
            {
                proxy.TestCommunication();
                Console.WriteLine("TestCommunication() finished. Press <enter> to continue ...");

                X509Certificate2 certificateSign = CertManager.GetCertificateFromStorage(StoreName.My,
                   StoreLocation.LocalMachine, signCertCN); ///!!nadje sertf

                Random r = new Random();
                string alarmMessage = AlarmMessage.GetAlarmMessage;
                while (true)
                {
                    int risk = r.Next(1, 100);

                    AlarmType alarmType = GetAlarmTypeForRisk(risk); //vraca tip alarma 

                    //string alarm = String.Format(alarmMessage, DateTime.Now, alarmType, risk);
                    string cao = "cao";
                    Console.WriteLine(cao);
                    Console.ReadKey();
                    byte[] signature = null;

                    //byte[] signature = DigitalSignature.Create(alarm, HashAlgorithm.SHA1, certificateSign);
                    //proxy.SendDataToEngine(alarm, signature); //1
                    proxy.SendDataToEngine(cao, signature);
                    //proxy.SendDataToEngine(alarm, signature); //1
                    Thread.Sleep(5000);

                }
                

            }

        }

        public static AlarmType GetAlarmTypeForRisk(int risk)
        {
            if (risk >= 0 && risk <= 20)
            {
                return AlarmType.NO_ALARM;
            }
            else if (risk >= 21 && risk <= 40)
            {
                return AlarmType.FALSE_ALARM;
            }
            else if (risk >= 41 && risk <= 60)
            {
                return AlarmType.INFO;
            }
            else if (risk >= 61 && risk <= 80)
            {
                return AlarmType.WARNING;
            }
            else
            {
                return AlarmType.ERROR;
            }
        }
    }
}
