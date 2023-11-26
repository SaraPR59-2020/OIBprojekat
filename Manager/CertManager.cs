using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;

namespace PubSubEngine
{
    public class CertManager
    {
        /// <summary>
		/// Get a certificate with the specified subject name from the predefined certificate storage
		/// Only valid certificates should be considered
		/// </summary>
		/// <param name="storeName"></param>
		/// <param name="storeLocation"></param>
		/// <param name="subjectName"></param>
		/// <returns> The requested certificate. If no valid certificate is found, returns null. </returns>
		public static X509Certificate2 GetCertificateFromStorage(StoreName storeName, StoreLocation storeLocation, string subjectName)
        {
            X509Store store = new X509Store(storeName, storeLocation);
            store.Open(OpenFlags.ReadOnly);

            X509Certificate2Collection certCollection = store.Certificates.Find(X509FindType.FindBySubjectName, subjectName, true);

            /// Check whether the subjectName of the certificate is exactly the same as the given "subjectName"
            foreach (X509Certificate2 c in certCollection)
            {
                if (c.SubjectName.Name.Equals(string.Format("CN={0}", subjectName)))
                {
                    return c;
                }
            }

            return null;
        }

     


        /*public static void CreateSelfSignedSertifikate()
        {
            srvCer = null;
            // Generate a new RSA key pair
            using (RSA rsa = RSA.Create())
            {
                // Create a certificate request with the RSA key pair
                CertificateRequest request = new CertificateRequest("CN=pubsubCN1", rsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

                // Set additional properties of the certificate
                request.CertificateExtensions.Add(
                    new X509BasicConstraintsExtension(false, false, 0, true));

                request.CertificateExtensions.Add(
                    new X509EnhancedKeyUsageExtension(
                        new OidCollection { new Oid("1.3.6.1.5.5.7.3.1") }, true));

                // Set the validity period of the certificate
                DateTimeOffset notBefore = DateTimeOffset.UtcNow;
                DateTimeOffset notAfter = notBefore.AddYears(1);

                // Create a self-signed certificate from the certificate request
                X509Certificate2 certificate = request.CreateSelfSigned(notBefore, notAfter);

                // Save the certificate to a file
                string certFilePath = "C:\\Program Files (x86)\\Windows Kits\\10\\bin\\10.0.19041.0\\x86\\pubsubCN1.pfx";
                string certPassword = "1234"; // Set a password to protect the private key
                File.WriteAllBytes(certFilePath, certificate.Export(X509ContentType.Pfx, certPassword));

                Console.WriteLine("Self-signed certificate created successfully.");
                Console.WriteLine($"Certificate saved to: {certFilePath}");
                srvCer = "pubsubCN1.pfx";
            }
        }*/

    }
}
