using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using System.ServiceModel;
using Manager;
using System.Runtime.InteropServices;
using System.Reflection;

namespace PubSubEngine
{
    [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    public class PubSubService : IEngine
    {
        public void SendDataToEngine(string alarm, byte[] sign)
        {
            string publisherName = Formatter.ParseName(ServiceSecurityContext.Current.PrimaryIdentity.Name);

            //pub
            string startupPath = Path.Combine(Directory.GetParent(System.IO.Directory.GetCurrentDirectory()).Parent.Parent.FullName, "keyPubEng.txt");
            string kljuc = SecretKey.LoadKey(startupPath);

            string decryptedAlarm = "";
            AES.DecryptString(alarm, out decryptedAlarm, kljuc);

            string[] parts = decryptedAlarm.Split(' ');
            string at = parts[3]; //tip alarma ce se odrediti na osnovu ovog broja
            Console.WriteLine(at);
            AlarmType alarmType = (AlarmType)Enum.Parse(typeof(AlarmType), at);
            Console.WriteLine(alarmType);

            //sub
            string startupPathSub = Path.Combine(Directory.GetParent(System.IO.Directory.GetCurrentDirectory()).Parent.Parent.FullName, "keySubEng.txt");
            byte[] publisherNameBytes = Encoding.ASCII.GetBytes(publisherName);

            foreach (Subscriber s in Base.Subscribers.Values)
            {
                try
                {
                    X509Certificate2 subscriberCert = CertManager.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, s.SubscriberName);
                    RSACryptoServiceProvider csp = (RSACryptoServiceProvider)subscriberCert.PublicKey.Key;
                    foreach (AlarmType currentAlarm in s.Alarms)
                    {
                        Console.WriteLine("Processing alarm...");

                        Console.WriteLine($"Alarm Type: {currentAlarm}");

                        if (currentAlarm.Equals(alarmType))
                        {
                            string encryptedAlarm = "";
                            AES.EncryptString(decryptedAlarm, out encryptedAlarm, SecretKey.LoadKey(startupPathSub));

                            Console.WriteLine($"Subscriber Name: {s.SubscriberName}");
                            Console.WriteLine($"Factory: {s.Proxy}");

                            s.Proxy.SendDataToSubscriber(encryptedAlarm, sign, csp.Encrypt(publisherNameBytes, false)); //problem
                        }
                        else
                        {
                            Console.WriteLine("Error: Alarm types do not match.");
                        }
                    }

                }
                catch (NullReferenceException ex)
                {
                    // Handle the NullReferenceException
                    Console.WriteLine($"Null reference exception: {ex.Message}");
                }
                catch (Exception ex)
                {
                    // Handle other exceptions
                    Console.WriteLine($"An error occurred: {ex.Message}");
                }
            }
        }
        public void Subscribe(string alarmTypes, string clientAddress)
        {
            string subscriberName = Formatter.ParseName(ServiceSecurityContext.Current.PrimaryIdentity.Name);
            string startupPathSub = Path.Combine(Directory.GetParent(System.IO.Directory.GetCurrentDirectory()).Parent.Parent.FullName, "keySubEng.txt");
            string decryptedAddress = "";
                AES.DecryptString(clientAddress, out decryptedAddress, SecretKey.LoadKey(startupPathSub));
            Console.WriteLine("dekriptovana: " + decryptedAddress);
            string decryptedAlarmTypes = "";
                AES.DecryptString(alarmTypes, out decryptedAlarmTypes, SecretKey.LoadKey(startupPathSub));
            Console.WriteLine("dat" + decryptedAlarmTypes);
            string[] parts = decryptedAlarmTypes.Trim().Split(' ');

            List<AlarmType> alarmTypess = new List<AlarmType>();
            string last = parts.Last(); //ovo mora jer trim ne uradi to sto mu treba PA NAM POJEDE SLEDECI
            //Console.WriteLine("last" + last); OVDE ON U STVARI ISPISE PRAYNO
            foreach (string part in parts)
            {
                if (!part.Equals(last))
                {
                    if (part == "")
                    {
                        break;
                    }
                    Console.WriteLine("ispis" + (AlarmType)Enum.Parse(typeof(AlarmType), part));
                    alarmTypess.Add((AlarmType)Enum.Parse(typeof(AlarmType), part));
                }
            }

            //pravimo kanal sa subskrajbovanim klijentom da moze da posalje poruku
            NetTcpBinding binding = new NetTcpBinding();
            using (ClientProxy pr = new ClientProxy(binding, decryptedAddress))
            {
                Subscriber s = new Subscriber(alarmTypess, pr, subscriberName);
                Console.WriteLine("PROXYYYYY" + s.Proxy.ToString());
                Base.Subscribers.TryAdd(decryptedAddress, s);
                Console.WriteLine("New subscriber!");
            }

        }
        public void TestCommunication()
        {
            Console.WriteLine("Communication established.");
        }
        public void Unsubscribe(string clientAddress)
        {
            Subscriber ret;
            Base.Subscribers.TryRemove(clientAddress, out ret);

            Console.WriteLine("New unsubscriber!");
        }
    }
}
