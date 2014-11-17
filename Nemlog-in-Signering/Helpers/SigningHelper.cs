using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace Schultz.Nemlogin.Signing.Helpers
{
    public class SigningHelper
    {
        public static SigningRequest BuildRequest(string id, string signText, string targetUrl)
        {
            var request = new SigningRequest()
            {
                SignText = Convert.ToBase64String(Encoding.UTF8.GetBytes(signText)),
                EntitId = SigningConfiguration.Instance.EntityId,
                RequestId = id,
                TargetUrl = targetUrl,
                DigestAlgorithm = "SHA1",
                SignTextFormat = "HTML",
                SigneringEndPoint = SigningConfiguration.Instance.SigningServiceUrl
            };

            // Note that this implementation relies on a signing certificate being configured to Nemlog-in SSO
            var certificate = SigningConfiguration.Instance.SigningCertificate;

            // Generate digest and sign
            var digest = string.Concat(request.SignText, request.EntitId, request.TargetUrl);
            var digestBytes = Encoding.UTF8.GetBytes(digest);
            var key = (RSACryptoServiceProvider)certificate.PrivateKey;
            var hashAlgo = CryptoConfig.CreateFromName(request.DigestAlgorithm);
            var signedFingerprint = key.SignData(digestBytes, hashAlgo);
            request.SignedFingerprint = Convert.ToBase64String(signedFingerprint);

            return request;
        }

        public static bool ValidateResponse(SigneringResponse response, string requestId, string signText)
        {
            if (response.Status.ToUpperInvariant() != "OK")
            {

                if (response.Status.ToUpperInvariant() == "CANCELLED")
                {
                    return false;
                }

                throw new DigitalSigneringFailedException(String.Format("An error occured, code {0}", response.Status));
            }

            if (response.RequestId != requestId)
            {
                throw new DigitalSigneringFailedException(String.Format("RequestId does not match expected value. expected: {0}, actual:{1}", requestId, response.RequestId));
            }

            if (String.IsNullOrEmpty(response.SignedSignatureProof))
            {
                throw new DigitalSigneringFailedException(String.Format("The response did not contain a signature proof. expected: {0}, actual:{1}", signText, response.SignedSignatureProof));
            }

            var recievedSignText = GetSignText(response.SignedSignatureProof);
            if (recievedSignText != signText)
            {
                throw new DigitalSigneringFailedException(String.Format("The signtext did not match the expected value. expected: {0}, actual:{1}", signText, recievedSignText));
            }

            var cert = GetCertificate(response);
            var expectedCertificateSubject = SigningConfiguration.Instance.SigningServiceCertificateSubject;
            if (!cert.Verify() && cert.SubjectName.Name != expectedCertificateSubject)
                throw new DigitalSigneringFailedException(String.Format("Certificate used for signing of signing response not valid. Certificate subject: {0}", cert.SubjectName.Name));

            var calculatedFingerprint = string.Concat(response.RequestId, response.Status, response.EntityId, response.PID,
                                            response.CVR, response.RID, response.SignedSignatureProof);
            var key = (RSACryptoServiceProvider)cert.PublicKey.Key;

            var signatureValid = key.VerifyData(Encoding.UTF8.GetBytes(calculatedFingerprint), CryptoConfig.CreateFromName("SHA256"), Convert.FromBase64String(response.SignedFingerPrint));
            if (!signatureValid)
            {
                throw new DigitalSigneringFailedException("Signature could not be verified");
            }

            return true;
        }

        private static X509Certificate2 GetCertificate(SigneringResponse response)
        {
            var signaturBevis = Encoding.UTF8.GetString(Convert.FromBase64String(response.SignedSignatureProof));

            var doc = new XmlDocument() { PreserveWhitespace = true };
            doc.LoadXml(signaturBevis);
            var signedXml = new SignedXml(doc);

            var nodeList = doc.GetElementsByTagName("Signature");
            signedXml.LoadXml((XmlElement)nodeList[0]);

            var cert = (X509Certificate2)signedXml.Signature.KeyInfo.Cast<KeyInfoX509Data>().First().Certificates[0];
            return cert;
        }

        private static string GetSignText(string signedSignatureProof)
        {
            var signaturBevis = Encoding.UTF8.GetString(Convert.FromBase64String(signedSignatureProof));

            var oons = XNamespace.Get("http://www.openoces.org/2006/07/signature#");
            var dsns = XNamespace.Get("http://www.w3.org/2000/09/xmldsig#");

            return XDocument.Parse(signaturBevis).Descendants(dsns + "SignatureProperty").Where(x => x.Element(oons + "Name").Value == "signtext").Elements(oons + "Value").Single().Value;
        }
    }

    public class SigneringResponse
    {
        public string RequestId { get; set; }
        public string Status { get; set; }
        public string EntityId { get; set; }
        public string PID { get; set; }
        public string CVR { get; set; }
        public string RID { get; set; }
        public string SignedSignatureProof { get; set; }
        public string SignedFingerPrint { get; set; }
    }

    /// <summary>
    /// Exception thrown if digital signering fails
    /// </summary>
    public class DigitalSigneringFailedException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DigitalSigneringFailedException"/> class.
        /// </summary>
        public DigitalSigneringFailedException()
            : base() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="DigitalSigneringFailedException"/> class.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public DigitalSigneringFailedException(string message)
            : base(message) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="DigitalSigneringFailedException"/> class.
        /// </summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
        public DigitalSigneringFailedException(string message, Exception innerException)
            : base(message, innerException) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="DigitalSigneringFailedException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext" /> that contains contextual information about the source or destination.</param>
        public DigitalSigneringFailedException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }
    }
}
