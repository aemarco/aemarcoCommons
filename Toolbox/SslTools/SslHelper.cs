using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;

namespace aemarcoCommons.Toolbox.SslTools
{
    public static class SslHelper
    {

        /// <summary>
        /// ensures given certificate is installed, and returns the thumbprint
        /// </summary>
        /// <param name="file">x509Certificate2 file</param>
        /// <param name="password">password for the certificate</param>
        /// <returns>thumbprint</returns>
        public static string EnsureCertificateInstalled(string file, string password)
        {
            if (!File.Exists(file))
                throw new FileNotFoundException("Could not find cert file");
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("password must be provided", nameof(password));

            var cert = new X509Certificate2(file, password, X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.MachineKeySet);
            if (cert.Thumbprint == null)
                throw new ApplicationException("Could not get thumbprint from cert");

            var store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
            store.Open(OpenFlags.ReadWrite);
            var storeResults = store.Certificates.Find(X509FindType.FindByThumbprint, cert.Thumbprint, false);
            //install cert
            if (storeResults.Count == 0)
            {
                store.Add(cert);
            }
            store.Close();
            store.Dispose();

            return cert.Thumbprint;
        }
    }
}
