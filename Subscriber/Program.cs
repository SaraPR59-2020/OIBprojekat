using Common;
using Manager;
using PubSubEngine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;

namespace Subscriber
{
    public class Program
    {
        static void Main(string[] args)
        {
            NetTcpBinding bindingSubForEngine = new NetTcpBinding();

            ServiceHost hostSubForEngine = new ServiceHost(typeof(SubForEngine));

            //on hostuje mesto na kom prima podatke
            //string addressSubForEngine = "net.tcp://localhost:4001/ISubForEngine";
            string addressSubForEngine = hostSubForEngine.BaseAddresses.First().ToString();
            hostSubForEngine.AddServiceEndpoint(typeof(ISubForEngine), bindingSubForEngine, addressSubForEngine);

            //audit
            Audit audit = new Audit();
            ServiceSecurityAuditBehavior newAudit = new ServiceSecurityAuditBehavior();
            newAudit.AuditLogLocation = AuditLogLocation.Application;
            newAudit.ServiceAuthorizationAuditLevel = AuditLevel.SuccessOrFailure;

            hostSubForEngine.Description.Behaviors.Remove<ServiceSecurityAuditBehavior>();
            hostSubForEngine.Description.Behaviors.Add(newAudit);

            hostSubForEngine.Open();

            //veza za sertifikat
            string pubsubCN = "PubSubEngine";

            NetTcpBinding binding = new NetTcpBinding();
            binding.Security.Transport.ClientCredentialType = TcpClientCredentialType.Certificate;

            /// Use CertManager class to obtain the certificate based on the "srvCertCN" representing the expected service identity.
            X509Certificate2 srvCert = CertManager.GetCertificateFromStorage(StoreName.Root, StoreLocation.LocalMachine, pubsubCN);
            EndpointAddress address = new EndpointAddress(new Uri("net.tcp://localhost:4001/PubSubService"),
                                      new X509CertificateEndpointIdentity(srvCert));

            //using (SubscriberClient proxy = new SubscriberClient(binding, address))
            //{
            SubscriberClient proxy = new SubscriberClient(binding, address);
                /// 1. Communication test
                proxy.TestCommunication();
                Console.WriteLine("TestCommunication() finished. Press <enter> to continue ...");

                while (true)
                {
                    try
                    {
                        // tip podatka za topik je alarm - znaci alarm se deli i salje medjusobno - ne trebaju klase topik pbliser i subskrajber!!!
                        List<AlarmType> alarmTypes = new List<AlarmType>();
                        int alarmType;

                    Console.WriteLine("Izaberite alarme na koje želite da se pretplatite.");
                    Console.WriteLine("Kada izaberete sve alarme na koje želite da se pretplatite, unesite 6.\n");

                    do
                    {
                            Console.WriteLine("Tip alarma za unos:" +
                                "\n " +
                                "1. BEZ_ALARMA\n " +
                                "2. LAŽNI_ALARM\n " +
                                "3. INFORMACIJA\n " +
                                "4. UPOZORENJE\n " +
                                "5. GREŠKA\n"
                                );

                            if (!Int32.TryParse(Console.ReadLine(), out alarmType))
                            {
                                Console.WriteLine("Pogresan unos.");
                                continue;
                            }

                            if (alarmType == 6) break;


                            if (alarmType < 1 || alarmType > 5)
                            {
                                Console.WriteLine("Pogresan unos.");
                                continue;
                            }

                            if (alarmTypes.Contains((AlarmType)alarmType - 1))
                            {
                                Console.WriteLine("Vec ste odabrali taj alarm.");
                                continue;
                            }

                            alarmTypes.Add((AlarmType)alarmType - 1);

                        } while (true);

                        string alarmTypess = "";
                        foreach (AlarmType at in alarmTypes)
                        {
                            alarmTypess = alarmTypess + at + " ";
                        }

                        string key = SecretKey.GenerateKey();
                        string startupPath = Path.Combine(Directory.GetParent(System.IO.Directory.GetCurrentDirectory()).Parent.Parent.FullName, "keySubEng.txt");
                        SecretKey.StoreKey(key, startupPath);

                        string encryptedAlarmTypes = "";
                        AES.EncryptString(alarmTypess, out encryptedAlarmTypes, key);
                        string encryptedAddressForEngine = "";
                        AES.EncryptString(addressSubForEngine, out encryptedAddressForEngine, key);

                        proxy.Subscribe(encryptedAlarmTypes, encryptedAddressForEngine); 

                        Console.WriteLine("Pritisnite x za gasenje:");

                        if (Console.ReadLine() == "x")
                        {
                            break;
                        }

                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("[ERROR] {0}", e.Message);
                        Console.WriteLine("[StackTrace] {0}", e.StackTrace);
                    }
                }
                proxy.Unsubscribe(addressSubForEngine);
           // }

        }
    }
}
