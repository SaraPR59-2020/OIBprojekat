﻿using Common;
using PubSubEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Manager;
using System.IO;
using System.ServiceModel;
using System.Collections;

namespace Subscriber
{
    [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    public class SubForEngine : ISubForEngine
    {
        public void SendDataToSubscriber(string alarm, byte[] sign, byte[] publisherName)
        {
            //sertf od subs
            string srvCertCN = Formatter.ParseName(WindowsIdentity.GetCurrent().Name);
            X509Certificate2 subscriberCert = CertManager.GetCertificateFromStorage(StoreName.My, StoreLocation.LocalMachine, srvCertCN);

            //kljuc od sertf i dekriptuje publisherName
            RSACryptoServiceProvider csp = (RSACryptoServiceProvider)subscriberCert.PrivateKey;
            byte[] publisherNameBytes = csp.Decrypt(publisherName, false);

            //niz bajtova pretvara u string
            string publisherNamee = Encoding.ASCII.GetString(publisherNameBytes);

            string publisherNameSign = publisherNamee + "_sign";
            X509Certificate2 certificate = CertManager.GetCertificateFromStorage(StoreName.TrustedPeople,
                StoreLocation.LocalMachine, publisherNameSign); //za dp

            X509Certificate2 certificate2 = CertManager.GetCertificateFromStorage(StoreName.My,
                StoreLocation.LocalMachine, publisherNamee); //za komunikaciju

            string subKeyPath = Path.Combine(Directory.GetParent(System.IO.Directory.GetCurrentDirectory()).Parent.Parent.FullName, "keySubEng.txt");
            string kljuc = SecretKey.LoadKey(subKeyPath);
            string decryptedAlarm = "";
            AES.DecryptString(alarm, out decryptedAlarm, kljuc);

                if (!DigitalSignature.Verify(decryptedAlarm, Manager.HashAlgorithm.SHA1, sign, certificate))
                {
                    Console.WriteLine("Sign is valid");
                    Console.WriteLine(decryptedAlarm);

                    try
                    {
                        int count;
                        try
                        {
                            count = File.ReadAllLines("database.txt").Length;
                        }
                        catch (FileNotFoundException)
                        {
                            count = 0;
                        }

                        StreamWriter sw = new StreamWriter("database.txt", true); //upis
                        sw.WriteLine("ID: {0} " + decryptedAlarm.ToString(), count + 1); //tip alarma
                        sw.Close();

                        try
                        {
                            UnicodeEncoding encoding = new UnicodeEncoding();
                            string str = encoding.GetString(sign);
                            Audit.NewDataStored(DateTime.Now.ToString(), "database.txt", count + 1, str, certificate2.GetPublicKeyString());
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }

                }
                else
                {
                    Console.WriteLine("Sign is invalid");
                }
        }
    }
    
}
