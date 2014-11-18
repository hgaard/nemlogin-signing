namespace Schultz.Nemlogin.Signing.Models
{
    public class SigneringResponse
    {
        public string RequestId { get; set; }
        public string Status { get; set; }
        public string EntityId { get; set; }
        public string Pid { get; set; }
        public string Cvr { get; set; }
        public string Rid { get; set; }
        public string SignedSignatureProof { get; set; }
        public string SignedFingerPrint { get; set; }
    }
}