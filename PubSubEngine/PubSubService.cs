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

namespace PubSubEngine
{
    public class PubSubService : IEngine
    { 
        //3
        public void SendDataToEngine(string alarm, byte[] sign)
        {
            //Console.WriteLine("ovde je");
            string publisherName = Formatter.ParseName(ServiceSecurityContext.Current.PrimaryIdentity.Name);
            //Console.WriteLine("Korisnik je: " + publisherName);

            //pub
            //Console.WriteLine(Directory.GetParent(System.IO.Directory.GetCurrentDirectory()).Parent.Parent.FullName);
            string startupPath = Path.Combine(Directory.GetParent(System.IO.Directory.GetCurrentDirectory()).Parent.Parent.FullName, "keyPubEng.txt");
            //Console.WriteLine(startupPath);
            string kljuc = AES.SecretKey.LoadKey(startupPath);
            Console.WriteLine("Kljuc izgleda ovako: ");
            Console.WriteLine(kljuc);

            // Console.WriteLine("Dekripcija se desava ovde: ");


            //string decrypedAlarm = AES.Decryption.DecryptString(alarm, kljuc);
            //probati cao
            //string decrypedAlarm = AES.Decryption.DecryptString(Encoding.ASCII.GetBytes(alarm), kljuc);
            string decrypedString = AES.Decryption.DecryptString(Encoding.ASCII.GetBytes(alarm), kljuc);
            Console.WriteLine("Dekriptovan string:");
            //Console.WriteLine(decrypedAlarm);
            Console.WriteLine(decrypedString);

            /*string[] parts = decrypedAlarm.Split(' ');
            string at = parts[0]; //tip alarma ce se odrediti na osnovu ovog broja


            Console.WriteLine(at.ToString());
            AlarmType alarmType = (AlarmType)Enum.Parse(typeof(AlarmType), at); //kastuje u enum id obijem tip alarma*/


            //sub
            Console.WriteLine(Directory.GetParent(System.IO.Directory.GetCurrentDirectory()).Parent.Parent.FullName);
            string startupPathSub = Path.Combine(Directory.GetParent(System.IO.Directory.GetCurrentDirectory()).Parent.Parent.FullName, "keySubEng.txt");

            //UnicodeEncoding encoding = new UnicodeEncoding();
            byte[] publisherNameBytes = Encoding.ASCII.GetBytes(publisherName);

            foreach (Subscriber s in Base.subscribers.Values)
            {
               X509Certificate2 subscriberCert = CertManager.GetCertificateFromStorage(StoreName.TrustedPeople, StoreLocation.LocalMachine, s.SubscriberName);
               RSACryptoServiceProvider csp = (RSACryptoServiceProvider)subscriberCert.PublicKey.Key;

               foreach (AlarmType alarmTypee in s.Alarms)
               {
                   /* if (alarmTypee.Equals(alarmType))
                    {
                        s.Proxy.SendDataToSubscriber(AES.Encryption.EncryptString(decrypedAlarm, AES.SecretKey.LoadKey(startupPathSub)),
                                                    sign, csp.Encrypt(publisherNameBytes, false));
                    }*/
               }
            }

        }

        public void Subscribe(string alarmTypes, string clientAddress)
        {
           /* string subscriberName = Formatter.ParseName(ServiceSecurityContext.Current.PrimaryIdentity.Name);
            Console.WriteLine(Directory.GetParent(System.IO.Directory.GetCurrentDirectory()).Parent.Parent.FullName);
            string startupPathSub = Path.Combine(Directory.GetParent(System.IO.Directory.GetCurrentDirectory()).Parent.Parent.FullName, "keySubEng.txt");
            //string decryptedAddress = AES.Decryption.DecryptString(Encoding.ASCII.GetBytes(clientAddress), AES.SecretKey.LoadKey(startupPathSub));

            string decryptedAlarmTypes = AES.Decryption.DecryptString(Encoding.ASCII.GetBytes(alarmTypes), AES.SecretKey.LoadKey(startupPathSub));

            string[] parts = decryptedAlarmTypes.Split(' ');

            List<AlarmType> alarmTypess = new List<AlarmType>();

            foreach (string at in parts)
            {
                if (at == "")
                {
                    break;
                }
                alarmTypess.Add((AlarmType)Enum.Parse(typeof(AlarmType), at));
            }


            NetTcpBinding binding = new NetTcpBinding();
            ClientProxy pr = new ClientProxy(binding, decryptedAddress);

            Subscriber s = new Subscriber(alarmTypess, pr, subscriberName);

            Base.subscribers.TryAdd(decryptedAddress, s);

            Console.WriteLine("New subscriber!"); */
        }

        public void TestCommunication()
        {
            Console.WriteLine("Communication established.");
        }

        public void Unsubscribe(string clientAddress)
        {
            Subscriber ret;
            Base.subscribers.TryRemove(clientAddress, out ret);

            Console.WriteLine("New unsubscriber!");
        }
    }
}
