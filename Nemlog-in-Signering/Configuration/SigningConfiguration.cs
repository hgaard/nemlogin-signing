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

        [ConfigurationProperty("signingCertificateThumbprint", IsRequired = true)]
        public string SigningCertificateThumbprint
        {
            get { return (string)this["signingCertificateThumbprint"]; }
            set { this["signingCertificateThumbprint"] = value; }
        }

        
    }
}
