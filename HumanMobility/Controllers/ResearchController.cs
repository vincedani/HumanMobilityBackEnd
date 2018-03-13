using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using DataAnalyzer.Entities;
using HumanMobility.DataAnalyzation;
using HumanMobility.Helpers;
using HumanMobility.Models;
using HumanMobility.Models.Services;
using Microsoft.AspNet.Identity;

namespace HumanMobility.Controllers
{
    [Authorize(Roles = Migrations.Configuration.AdminRole)]
    public class ResearchController : Controller
    {
        #region Members
        private readonly IService _service;
        #endregion

        #region Constructor
        public ResearchController(IService service)
        {
            _service = service;
        }
        #endregion

        #region Controller functions
        // GET: Research
        public ActionResult Index()
        {
            ViewBag.Users = _service.GetUserNames().OrderBy(s => s);
            return View();
        }

        // POST: Research
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(ResearchViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Users = _service.GetUserNames().OrderBy(s => s);
                return View();
            }
            ModelState.Clear();
            if (model.From > model.To)
            {
                ModelState.AddModelError("From", @"The first date must be less than second");
                ModelState.AddModelError("To", @"The first date must be less than second");
                ViewBag.Users = _service.GetUserNames().OrderBy(s => s);
                return View();
            }

            string exportedUserId = _service.GetUserId(model.UserName);
            string report = ReportHelper.GetData(model, _service, User.Identity.GetUserId(), exportedUserId);
            string fileName = string.Concat("Report-location-", model.UserName, "-", DateTime.Now, ".txt");

            return File(Encoding.UTF8.GetBytes(report), "text/plain", fileName);
        }

        // POST: Summary
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Summary(SummaryViewModel model)
        {
            if (!ModelState.IsValid || model == null)
            {
                ViewBag.Users = _service.GetUserNames().OrderBy(s => s);
                return View("Index");
            }

            ModelState.Clear();
            int days = Convert.ToInt32((DateTime.Today - model.FromDate).TotalDays);
            if (days < 0)
            {
                ViewBag.Users = _service.GetUserNames().OrderBy(s => s);
                ModelState.AddModelError("FromDate", @"The date can't be in the future.");
                return View("Index");
            }

            string summary = ReportHelper.GetSummary(_service, days, model.FromDate);
            string fileName = string.Concat("Summary-", DateTime.Now, ".txt");
            return File(Encoding.UTF8.GetBytes(summary), "text/plain", fileName);
        }

        public ActionResult Actigraphy(ResearchAccelerometerViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Users = _service.GetUserNames().OrderBy(s => s);
                return View("Index");
            }

            ModelState.Clear();
            if (model._From > model._To)
            {
                ModelState.AddModelError("_From", @"The first date must be less than second");
                ModelState.AddModelError("To", @"The first date must be less than second");
                ViewBag.Users = _service.GetUserNames().OrderBy(s => s);
                return View();
            }

            string fileName, report;
            string exportedUserId = _service.GetUserId(model._UserName);
            var activities = BulkCommands.GetActivities(exportedUserId, model._From, model._To);
            List<ActivityPerMinute> activityPerMinute = null;

            switch (model._ActivityOptions)
            {
                case AcceptableActivityOptions.Export:
                    model._ExportOptions = AcceptableExportOptions.Text;
                    fileName = "Report";
                    report = ReportHelper.GetAccelerometerData(activities);
                    break;

                case AcceptableActivityOptions.StandardDeviation:
                    model._ExportOptions = AcceptableExportOptions.Text;
                    fileName = "StdDev";
                    double devX = StandardDeviation.CalculateStdDev(activities.Select(m => (double) m.X).ToList());
                    double devY = StandardDeviation.CalculateStdDev(activities.Select(m => (double) m.Y).ToList());
                    double devZ = StandardDeviation.CalculateStdDev(activities.Select(m => (double) m.Z).ToList());
                    report = ReportHelper.ExportStdDev(devX, devY, devZ);
                    break;

                case AcceptableActivityOptions.EnergyExpenditure:
                case AcceptableActivityOptions.ZeroCrossing:
                    activityPerMinute = CalculateActivity.BeginAnalyzation(model._ActivityOptions, activities);
                    report = ReportHelper.ExportProcessedData(activityPerMinute);
                    fileName = string.Concat("Processed-", model._ActivityOptions);
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            activities.Clear();
            GC.Collect();

            switch (model._ExportOptions)
            {
                case AcceptableExportOptions.Chart:
                    var chart = ReportHelper.CreateChart(activityPerMinute);

                    // ReSharper disable once PossibleNullReferenceException
                    activityPerMinute.Clear();
                    GC.Collect();

                    return File(chart.GetBytes(), "jpeg",
                        string.Concat(fileName, "-accelerometer - ", model._UserName, " - ", DateTime.Now, ".jpeg"));

                case AcceptableExportOptions.Text:
                    return File(Encoding.UTF8.GetBytes(report), "text/plain", 
                        string.Concat(fileName, "-accelerometer - ", model._UserName, " - ", DateTime.Now, ".txt"));

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        #endregion
    }
}
