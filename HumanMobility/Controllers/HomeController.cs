using System;
using System.Text;
using System.Web.Mvc;
using HumanMobility.Helpers;
using HumanMobility.Models;
using HumanMobility.Models.Services;
using Microsoft.AspNet.Identity;

namespace HumanMobility.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        #region Members
        private readonly IService _service;
        #endregion

        #region Constructor
        public HomeController(IService service)
        {
            _service = service;
        }
        #endregion

        #region Controller functions
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult Contact()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult Terms()
        {
            return View();
        }
        #endregion

        #region HTTP functions
        [HttpPost]
        public ActionResult GetMyData(ResearchViewModel model)
        {
            if (!ModelState.IsValid || model == null)
                return RedirectToAction("Index");

            string userId = User.Identity.GetUserId();

            string report = ReportHelper.GetData(model, _service, userId, userId);
            string fileName = string.Concat("Report-", model.UserName, "-", DateTime.Now, ".txt");

            return File(Encoding.UTF8.GetBytes(report), "text/plain", fileName);
        }
        #endregion
    }
}
