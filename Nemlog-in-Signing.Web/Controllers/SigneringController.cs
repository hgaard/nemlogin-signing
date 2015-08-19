using System;
using System.Web.Mvc;
using System.Web.Routing;
using Hgaard.Nemlogin.Signing.Models;

namespace Hgaard.Nemlogin.Signing.Controllers
{
    public class SigningController : Controller
    {
        private const string Message = "<html>my message</html>";

        public ActionResult Index()
        {
            
            var messageId = Guid.NewGuid().ToString();
            var request = Signer.BuildRequest(messageId, Message, Url.Action("ValidateSigning", "Signing", new RouteValueDictionary() { { "id", messageId } }, Request.Url.Scheme, Request.Url.Host));
            return View(request);
        }

        [HttpPost]
        public ActionResult ValidateSigning(string id)
        {
            var response = new SigneringResponse()
            {
                RequestId = Request.Form["RequestId"],
                Status = Request.Form["Status"],
                EntityId = Request.Form["EntityId"],
                SignedSignatureProof = Request.Form["SignedSignatureProof"],
                SignedFingerPrint = Request.Form["SignedFingerPrint"],
                Pid = Request.Form["PID"],
                Cvr = Request.Form["CVR"],
                Rid = Request.Form["RID"]
            };

            var validationResult = Signer.ValidateResponse(response, id, Message);
           
            return RedirectToAction("Index", "Home", new RouteValueDictionary() { { "result", validationResult } }); 
        }
    }
}