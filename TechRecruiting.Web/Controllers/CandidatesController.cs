using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;
using TechRecruiting.Models;
using TechRecruiting.Web.Data;

namespace TechRecruiting.Web.Controllers
{
    [HandleError]
    public class CandidatesController : Controller
    {
        private CandidateData _candidateData;

        public CandidatesController(CandidateData candidateData)
        {
            _candidateData = candidateData;
        }

        [HttpGet]
        [Route("~/candidates", Name = "ListCandidates")]
        public async Task<ActionResult> Index()
        {
            IList<Candidate> model = await _candidateData.GetCandidates();
            return View(model);
        }

        [HttpGet]
        [Route("~/candidates/{id}", Name = "GetCandidate")]
        public async Task<ActionResult> Get(string id)
        {
            Candidate model = await _candidateData.GetCandidateWithFriendships(id);
            return View(model);
        }

        [HttpGet]
        [Route("~/candidates/create", Name = "CreateCandidate")]
        public ActionResult Add()
        {
            return View(new Candidate());
        }

        [HttpPost]
        [Route("~/candidates/create", Name = "PersistCandidate")]
        public async Task<ActionResult> Add(Candidate model)
        {
            await _candidateData.PersistCandidate(model);
            return RedirectToRoute("ListCandidates");
        }
    }
}