using AAAService.Models;
using System;
using System.Data.Entity;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Data;
using System.Data.Linq;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
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
            reportInformation.ReportName = "";
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
                                                        ColumnName = l.ColumnName,
                                                        Name = l.Name
                                                    }).ToList();

            reportInformation.ReportBuilderView = new List<ReportBuilder_View>();

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
        public ActionResult Index([Bind(Include = "ReportName, From, To, Companies, Locations, ReportBuilderFields, ReportBuilderView")] Models.ReportBuilderModel.ReportInformation reportInformation)
        {
            if (ModelState.IsValid)
            {
                var strDDLCompaniesValue = Request.Form["ddlCompanies"];
                var strDDLLocationsValue = Request.Form["ddlLocations"];
                var strDDLToValue = Request.Form["to[]"];

                if (strDDLToValue != null)
                {
                    var strDDLToValueSplitted = strDDLToValue.Split(',');
                    //var retrieveFieldNames = new StringBuilder();

                    //foreach (var item in strDDLToValueSplitted)
                    //{
                    //    retrieveFieldNames = retrieveFieldNames.Append(db.ReportBuilderFields
                    //                                                     .Where(rp => rp.Id.ToString() == item));
                    //}

                    //var query = "SELECT top 5 " + strDDLToValue + " FROM service_tickets";

                    //using (var context = new aaahelpEntities())
                    //{
                    //    var dynamicResults = context.Database.SqlQuery<int>(query).ToList();

                    //    foreach (int item in dynamicResults)
                    //    {
                    //        // treat as a particular type based on the position in the list
                    //    }
                    //}

                    reportInformation.ReportBuilderView = db.ReportBuilder_View.Where(rp => rp.order_datetime >= reportInformation.From && rp.order_datetime <= reportInformation.To).ToList<ReportBuilder_View>();
                    //GenerateExcelSpreadsheet(strDDLToValueSplitted, result);
                }             
            }

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
                                                        ColumnName = l.ColumnName,
                                                        Name = l.Name
                                                    }).ToList();

            TempData["ReportName"] = reportInformation.ReportName;
            TempData["From"] = reportInformation.From;
            TempData["To"] = reportInformation.To;

            return View(reportInformation);
        }

        //internal void GenerateExcelSpreadsheet(string[] columns, List<ReportBuilder_View> result)
        public ActionResult GenerateExcelSpreadsheet([Bind(Include = "ReportName, From, To, Companies, Locations, ReportBuilderFields, ReportBuilderView")] Models.ReportBuilderModel.ReportInformation reportInformation)
        {
            var reportName = TempData.Peek("ReportName");
            var from = DateTime.Parse(TempData.Peek("From").ToString());
            var to = DateTime.Parse(TempData.Peek("To").ToString());
            var reportBuilder = db.ReportBuilder_View.Where(rp => rp.order_datetime >= from && rp.order_datetime <= to).ToList();
            var grid = new GridView();
            grid.DataSource = reportBuilder;
            grid.DataBind();

            //grid.Font.Size = 8;
            //grid.BorderStyle = BorderStyle.None;
            //grid.Font.Name = "Arial";

            //grid.HeaderRow.Cells[0].Text = "Job Number";
            //grid.HeaderRow.Cells[0].Width = 69;

            for (var i = 0; i < 27; i++)
            {
                grid.HeaderRow.Cells[i].Text = "Column" + i;
                //grid.HeaderRow.Cells[i].Font.Bold = false;
                //grid.HeaderRow.Cells[i].VerticalAlign = VerticalAlign.Bottom;
            }

            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=Report" + DateTime.Now + ".xls");
            Response.ContentType = "application/ms-excel";
            Response.Charset = "";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            grid.RenderControl(htw);
            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();
            return View("View");
        }

        [HttpPost]
        public ActionResult GetServiceLocations(string companyGUID)
        {
            SelectList obj = new SelectList(db.locationinfoes.Where(o => o.parentguid.ToString() == companyGUID && o.active == true).OrderBy(o => o.name), "guid", "Name");
            return Json(obj);
        }
    }
}