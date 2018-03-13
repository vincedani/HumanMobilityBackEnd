using System.Web.Mvc;
using HumanMobility.Models;
using HumanMobility.Models.Services;
using Microsoft.AspNet.Identity;
using PagedList;

namespace HumanMobility.Controllers
{
    [Authorize]
    public class BugReportController : Controller
    {
        #region Members
        private readonly IService _service;
        #endregion

        #region Constructor
        public BugReportController(IService service)
        {
            _service = service;
        }
        #endregion

        // GET: BugReport
        public ActionResult Index(int? page)
        {
            var reports = _service.GetBugReports(User);
            int pageNumber = page ?? 1;

            return View(reports.ToPagedList(pageNumber, 15));
        }

        // POST: BugReport
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(BugReportViewModel model)
        {
            if (!ModelState.IsValid || model == null)
                return View();

            _service.WriteBugReport(User.Identity.GetUserId(), model.Message);
            return RedirectToAction("Index");
        }
    }
}
