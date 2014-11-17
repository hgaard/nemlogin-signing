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
            get { return (SigningConfiguration)ConfigurationManager.GetSection("kombit.bom.signering"); }
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

        [ConfigurationProperty("signingServiceCertificateSubject", IsRequired = true)]
        public string SigningServiceCertificateSubject
        {
            get { return (string)this["signingServiceCertificateSubject"]; }
            set { this["signingServiceCertificateSubject"] = value; }
        }


        // TODO!!!
        public X509Certificate2 SigningCertificate { get; set; }

    }
}
