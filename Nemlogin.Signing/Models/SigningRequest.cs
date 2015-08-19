using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hgaard.Nemlogin.Signing.Models
{
    public class SigningRequest
    {
        public string SignText { get; set; }
        public string SignTextFormat { get; set; }
        public string EntitId { get; set; }
        public string RequestId { get; set; }
        public string TargetUrl { get; set; }
        public string SignedFingerprint { get; set; }
        public string SerialNumber { get; set; }
        public string Headline { get; set; }
        public string Language { get; set; }
        public string DigestAlgorithm { get; set; }

        public string SigneringEndPoint { get; set; }
    }
}
