using System.Configuration;
using System.Security.Cryptography.X509Certificates;

namespace Schultz.Nemlogin.Signing.Configuration
{
    public class SigningConfiguration : ConfigurationSection
    {
        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>
        /// The instance.
        /// </value>
        public static SigningConfiguration Instance
        {
            get { return (SigningConfiguration)ConfigurationManager.GetSection("Nemlogin.Signing"); }
        }

        [ConfigurationProperty("entityId", IsRequired = true)]
        public string EntityId
        {
            get { return (string)this["entityId"]; }
            set { this["entityId"] = value; }
        }

        [ConfigurationProperty("signingServiceUrl", IsRequired = true)]
        public string SigningServiceUrl
        {
            get { return (string)this["signingServiceUrl"]; }
            set { this["signingServiceUrl"] = value; }
        }

        [ConfigurationProperty("signingAuthorityServiceCertificateSubject", IsRequired = true)]
        public string SigningAuthorityServiceCertificateSubject
        {
            get { return (string)this["signingAuthorityServiceCertificateSubject"]; }
            set { this["signingAuthorityServiceCertificateSubject"] = value; }
        }


        // TODO!!!
        public X509Certificate2 SigningCertificateThumbprint
        {
            get { return GetCertificate((string)this["signingCertificateThumbprint"]); }
                
            set { this["signingCertificateThumbprint"] = value; }
        }

         public X509Certificate2 GetCertificate(string thumbprint)
         {
             var store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
            try
            {
                store.Open(OpenFlags.ReadOnly);
                var found = store.Certificates.Find( X509FindType.FindByThumbprint, thumbprint, false);
                if (found.Count == 0)
                    throw new ConfigurationErrorsException("No certificate found");
                if (found.Count > 1)
                    throw new ConfigurationErrorsException("More than one certificate found");
                return found[0];
            }
            finally
            {
                store.Close();
            }
        }
    }
}
