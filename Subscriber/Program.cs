using AES;
using Common;
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
            string addressSubForEngine = "net.tcp://localhost:4040";
            hostSubForEngine.AddServiceEndpoint(typeof(ISubForEngine), bindingSubForEngine, addressSubForEngine);


            //audit
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

            using (SubscriberClient proxy = new SubscriberClient(binding, address))
            {
                /// 1. Communication test
                proxy.TestCommunication();
                Console.WriteLine("TestCommunication() finished. Press <enter> to continue ...");

                //Console.WriteLine("Uskoro cete imati uvid u dostupne teme ako ih ima...");
                ////string rezultat = proxy.AddSubscriber(new PubSubEngine.Subscriber());
                ////Console.WriteLine(rezultat);
                //Console.ReadLine();


                while (true)
                {
                    try
                    {
                        // tip podatka za topik je alarm - znaci alarm se deli i salje medjusobno - ne trebaju klase topik pbliser i subskrajber!!!
                        List<AlarmType> alarmTypes = new List<AlarmType>();
                        int alarmType;

                        Console.WriteLine("Izaberite alarme na koje zelite da se pretplatite.");
                        do
                        {
                            Console.WriteLine("Tip alarma za unos:" +
                                "\n " +
                                "1. NO_ALARM\n " +
                                "2. FALSE_ALARM\n " +
                                "3. INFO\n " +
                                "4. WARNING\n " +
                                "5. ERROR\n" +
                                "6. *prekid izbora*\n"
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
                            alarmTypess = alarmTypess + at + "+";
                        }

                        string key = AES.SecretKey.GenerateKey();
                        //Console.WriteLine(Directory.GetParent(System.IO.Directory.GetCurrentDirectory()).Parent.Parent.FullName);
                        string startupPath = Path.Combine(Directory.GetParent(System.IO.Directory.GetCurrentDirectory()).Parent.Parent.FullName, "keySubEng.txt");
                        SecretKey.StoreKey(key, startupPath);


                        proxy.Subscribe(AES.Encryption.EncryptString(alarmTypess, key),
                            AES.Encryption.EncryptString(addressSubForEngine, key)); //poslednja greska



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
            }
        }
    }
}
