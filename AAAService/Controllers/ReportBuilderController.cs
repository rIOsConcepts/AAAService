using AAAService.Models;
using System;
using System.Data.Entity;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace AAAService.Controllers
{
    [Authorize(Roles = "SiteAdmin")]
    public class ReportBuilderController : Controller
    {
        private aaahelpEntities db = new aaahelpEntities();

        public ReportBuilderController()
        {
        }

        public ActionResult Index()
        {
            var reportInformation = new Models.ReportBuilderModel.ReportInformation();
            reportInformation.ReportName = "Manuel Rios";
            reportInformation.From = DateTime.Now;
            reportInformation.To = DateTime.Now;

            reportInformation.Companies = db.Companies
                                            .Where(c => c.active)
                                            .Select(c => new Models.ReportBuilderModel.Company()
                                            {
                                                GUID = c.guid,
                                                Name = c.name
                                            })
                                            .OrderBy(c => c.Name).ToList();

            reportInformation.Locations = db.locationinfoes
                                            .Where(l => l.active)
                                            .Select(l => new Models.ReportBuilderModel.Location()
                                            {
                                                GUID = l.guid,
                                                Name = l.name
                                            })
                                            .OrderBy(l => l.Name).ToList();

            reportInformation.ReportBuilderFields = db.ReportBuilderFields
                                                    .Select(l => new Models.ReportBuilderModel.ReportBuilderField()
                                                    {
                                                        Id = l.Id,
                                                        Name = l.Name
                                                    }).ToList();

            //return View(new Tuple<Models.ReportBuilderModel.ReportInformation, IEnumerable<Models.ReportBuilderModel.Companies>, IEnumerable<Models.ReportBuilderModel.Locations>, IEnumerable<Models.ReportBuilderModel.ReportBuilderFields>>(reportInformation, companies, locations, reportBuilderFields));
            //return View(new Tuple<Models.ReportBuilderModel.ReportInformation, IEnumerable<Models.ReportBuilderModel.Companies>, IEnumerable<Models.ReportBuilderModel.ReportBuilderFields>>(reportInformation, companies, reportBuilderFields));
            return View(reportInformation);
        }

        //[HttpPost, ActionName("Index")]
        //[ValidateAntiForgeryToken]
        //public ActionResult GenerateExcel()
        //{
        //    if (ModelState.IsValid)
        //    {

        //    }

        //    return View();
        //}

        //[HttpPost, ActionName("Index")]
        [HttpPost]
        //[ChildActionOnly]
        [ValidateAntiForgeryToken]
        //public ActionResult Index([Bind(Include = "ReportName,From,To")] Tuple<Models.ReportBuilderModel.ReportInformation, IEnumerable<Models.ReportBuilderModel.Companies>, IEnumerable<AAAService.Models.ReportBuilderModel.ReportBuilderFields>> t)
        //public ActionResult GenerateExcel(Tuple<Models.ReportBuilderModel.ReportInformation, IEnumerable<Models.ReportBuilderModel.Companies>, IEnumerable<AAAService.Models.ReportBuilderModel.ReportBuilderFields>> t)
        //public ActionResult BuildReport([Bind(Include = "ReportName,From,To")] Tuple<Models.ReportBuilderModel.ReportInformation, IEnumerable<Models.ReportBuilderModel.Companies>, IEnumerable<Models.ReportBuilderModel.Locations>, IEnumerable<Models.ReportBuilderModel.ReportBuilderFields>> t)
        public ActionResult Index([Bind(Include = "ReportName,From,To,Companies,Locations,ReportBuilderFields")] Models.ReportBuilderModel.ReportInformation reportInformation)
        {
            if (ModelState.IsValid)
            {
                string strDDLValue = Request.Form[0].ToString();
                var toExcel = db.ReportBuilder("Select * from Regions");
                return View();
            }
            else
            {
                return View();
            }            
        }

        [HttpPost]
        public ActionResult GetServiceLocations(string companyGUID)
        {
            SelectList obj = new SelectList(db.locationinfoes.Where(o => o.parentguid.ToString() == companyGUID && o.active == true).OrderBy(o => o.name), "guid", "Name");
            return Json(obj);
        }
    }
}