using System.Web.Mvc;

namespace TechRecruiting.Web.Controllers
{
    [HandleError]
    public class HomeController : Controller
    {
        [HttpGet]
        [Route("~/", Name = "Home")]
        public ActionResult Index()
        {
            return View();
        }
    }
}