using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;
using TechRecruiting.Models;
using TechRecruiting.Web.Data;

namespace TechRecruiting.Web.Controllers
{
    [HandleError]
    public class RecruitersController : Controller
    {
        private RecruiterData _recruiterData;

        public RecruitersController(RecruiterData recruiterData)
        {
            _recruiterData = recruiterData;
        }

        [HttpGet]
        [Route("~/recruiters", Name = "ListRecruiters")]
        public async Task<ActionResult> Index()
        {
            IEnumerable<Recruiter> model = await _recruiterData.GetRecruiters();
            return View(model);
        }

        [HttpGet]
        [Route("~/recruiters/{id}", Name = "GetRecruiters")]
        public async Task<ActionResult> Get(string id)
        {
            Recruiter model = await _recruiterData.GetRecruiterWithCandidates(id);
            return View(model);
        }

        [HttpGet]
        [Route("~/recruiters/create", Name = "CreateRecruiter")]
        public ActionResult Add()
        {
            return View(new Recruiter());
        }

        [HttpPost]
        [Route("~/recruiters/create", Name = "PersistRecruiter")]
        public async Task<ActionResult> Add(Recruiter model)
        {
            await _recruiterData.PersistRecruiter(model);
            return RedirectToRoute("ListRecruiters");
        }
    }
}