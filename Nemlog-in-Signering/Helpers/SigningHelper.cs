using System;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Schultz.Nemlogin.Signing.Configuration;
using Schultz.Nemlogin.Signing.Models;

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
                DigestAlgorithm = "SHA256",
                SignTextFormat = "HTML",
                SigneringEndPoint = SigningConfiguration.Instance.SigningServiceUrl
            };

            // Note that this implementation relies on a signing certificate being configured to Nemlog-in SSO
            var certificate = GetCertificate(SigningConfiguration.Instance.SigningCertificateThumbprint);

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

            //if (response.RequestId != requestId)
            //{
            //    throw new DigitalSigneringFailedException(String.Format("RequestId does not match expected value. expected: {0}, actual:{1}", requestId, response.RequestId));
            //}

            //if (String.IsNullOrEmpty(response.SignedSignatureProof))
            //{
            //    throw new DigitalSigneringFailedException(String.Format("The response did not contain a signature proof. expected: {0}, actual:{1}", signText, response.SignedSignatureProof));
            //}

            //var recievedSignText = GetSignText(response.SignedSignatureProof);
            //if (recievedSignText != signText)
            //{
            //    throw new DigitalSigneringFailedException(String.Format("The signtext did not match the expected value. expected: {0}, actual:{1}", signText, recievedSignText));
            //}

            //var cert = GetCertificate(response);
            //var expectedCertificateSubject = SigningConfiguration.Instance.SigningAuthorityServiceCertificateSubject;
            //if (!cert.Verify() && cert.SubjectName.Name != expectedCertificateSubject)
            //    throw new DigitalSigneringFailedException(String.Format("Certificate used for signing of signing response not valid. Certificate subject: {0}", cert.SubjectName.Name));

            //var calculatedFingerprint = string.Concat(response.RequestId, response.Status, response.EntityId, response.Pid,
            //                                response.Cvr, response.Rid, response.SignedSignatureProof);
            //var key = (RSACryptoServiceProvider)cert.PublicKey.Key;

            //var signatureValid = key.VerifyData(Encoding.UTF8.GetBytes(calculatedFingerprint), CryptoConfig.CreateFromName("SHA256"), Convert.FromBase64String(response.SignedFingerPrint));
            //if (!signatureValid)
            //{
            //    throw new DigitalSigneringFailedException("Signature could not be verified");
            //}

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

        private static X509Certificate2 GetCertificate(string thumbprint)
        {
            var store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
            try
            {
                store.Open(OpenFlags.ReadOnly);
                var found = store.Certificates.Find(X509FindType.FindByThumbprint, thumbprint, false);
                if (found.Count == 0)
                    throw new ArgumentException("No certificate found");
                if (found.Count > 1)
                    throw new ArgumentException("More than one certificate found");
                return found[0];
            }
            finally
            {
                store.Close();
            }
        }
    }
}
