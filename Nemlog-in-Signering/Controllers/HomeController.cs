using System.Web.Mvc;

namespace Schultz.Nemlogin.Signing.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index(string result)
        {
            this.ViewBag.Id = result;
            return View();
        }
    }
}