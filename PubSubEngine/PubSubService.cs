using Contracts;
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

        public void SendDataToEngine(string alarm, byte[] sign)
        {
            string publisherName = Formatter.ParseName(ServiceSecurityContext.Current.PrimaryIdentity.Name);

            //pub
            Console.WriteLine(Directory.GetParent(System.IO.Directory.GetCurrentDirectory()).Parent.Parent.FullName);
            string startupPath = Path.Combine(Directory.GetParent(System.IO.Directory.GetCurrentDirectory()).Parent.Parent.FullName, "keyPubEng.txt");
            Console.WriteLine(startupPath);
            string str = AES.SecretKey.LoadKey(startupPath);
            Console.WriteLine(str);
            string decrypedAlarm = AES.Decryption.DecryptString(alarm, str);
            Console.WriteLine("4"); //proslo nesto
            Console.WriteLine(decrypedAlarm);

            string[] parts = decrypedAlarm.Split(' ');
            string at = parts[6]; //tip alarma


            Console.WriteLine(at.ToString());
            AlarmType alarmType = (AlarmType)Enum.Parse(typeof(AlarmType), at); //kastuje u enum id obijem tip alarma

            //sub
            Console.WriteLine(Directory.GetParent(System.IO.Directory.GetCurrentDirectory()).Parent.Parent.FullName);
            string startupPathSub = Path.Combine(Directory.GetParent(System.IO.Directory.GetCurrentDirectory()).Parent.Parent.FullName, "keySubEng.txt");

            UnicodeEncoding encoding = new UnicodeEncoding();
            byte[] publisherNameBytes = encoding.GetBytes(publisherName);

            foreach (Subscriber s in Base.subscribers.Values)
            {
                X509Certificate2 subscriberCert = CertManager.GetCertificateFromStorage(StoreName.TrustedPeople, StoreLocation.LocalMachine, s.SubscriberName);
                RSACryptoServiceProvider csp = (RSACryptoServiceProvider)subscriberCert.PublicKey.Key;

                foreach (AlarmType alarmTypee in s.Alarms)
                {
                    if (alarmTypee.Equals(alarmType))
                    {
                        s.Proxy.SendDataToSubscriber(AES.Encryption.EncryptString(decrypedAlarm, AES.SecretKey.LoadKey(startupPathSub)),
                                                    sign, csp.Encrypt(publisherNameBytes, false));
                    }
                }
            }
        }

        public void Subscribe(string alarmTypes, string clientAddress)
        {
            string subscriberName = Formatter.ParseName(ServiceSecurityContext.Current.PrimaryIdentity.Name);
            Console.WriteLine(Directory.GetParent(System.IO.Directory.GetCurrentDirectory()).Parent.Parent.FullName);
            string startupPathSub = Path.Combine(Directory.GetParent(System.IO.Directory.GetCurrentDirectory()).Parent.Parent.FullName, "keySubEng.txt");
            string decryptedAddress = AES.Decryption.DecryptString(clientAddress, AES.SecretKey.LoadKey(startupPathSub));

            string decryptedAlarmTypes = AES.Decryption.DecryptString(alarmTypes, AES.SecretKey.LoadKey(startupPathSub));

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

            Console.WriteLine("New subscriber!");
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
