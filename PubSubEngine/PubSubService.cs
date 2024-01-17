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
                AlarmType alarmType = (AlarmType)Enum.Parse(typeof(AlarmType), at);

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
                            if (currentAlarm.Equals(alarmType))
                            {
                                string encryptedAlarm = "";
                                AES.EncryptString(decryptedAlarm, out encryptedAlarm, SecretKey.LoadKey(startupPathSub));
                                Console.WriteLine($"Sending {decryptedAlarm} to subscriber...");
                                s.Proxy.SendDataToSubscriber(encryptedAlarm, sign, csp.Encrypt(publisherNameBytes, false));
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
            string decryptedAlarmTypes = "";
                AES.DecryptString(alarmTypes, out decryptedAlarmTypes, SecretKey.LoadKey(startupPathSub));
            string[] parts = decryptedAlarmTypes.Trim().Split(' ');

            List<AlarmType> alarmTypess = new List<AlarmType>();

            string last = parts.Last(); //ovo mora jer trim ne uradi to sto mu treba PA NAM POJEDE SLEDECI
            foreach (string part in parts)
            {
                if (!part.Equals(last))
                {
                    if (part == "")
                    {
                        break;
                    }
                    alarmTypess.Add((AlarmType)Enum.Parse(typeof(AlarmType), part));
                }
            }

            //pravimo kanal sa subskrajbovanim klijentom da moze da posalje poruku
            NetTcpBinding binding = new NetTcpBinding();
            ClientProxy pr = new ClientProxy(binding, clientAddress);
            
            Subscriber s = new Subscriber(alarmTypess, pr, subscriberName);
            Base.Subscribers.TryAdd(clientAddress, s);
            Console.WriteLine("New subscriber!");
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
