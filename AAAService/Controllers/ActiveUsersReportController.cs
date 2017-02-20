using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Mvc;
using AAAService.Models;

namespace AAAService.Controllers
{
    [Authorize(Roles = "SiteAdmin")]
    public class ActiveUsersReportController : Controller
    {
        private aaahelpEntities db = new aaahelpEntities();

        public ActionResult Index()
        {
            var activeUsersReport = (from aur in db.ActiveUsersReportViews
                                     orderby aur.Email ascending
                                     select aur);

            var view = activeUsersReport.ToList<ActiveUsersReportView>();
            return View(view);
        }

        public ActionResult ExportToExcel()
        {
            var filteredRows = (List<ActiveUsersReportView>)Session["Items"];
            var grid = new GridView();
            grid.DataSource = filteredRows;
            grid.DataBind();
            grid.Font.Size = 8;
            grid.BorderStyle = BorderStyle.None;
            grid.Font.Name = "Arial";
            grid.HeaderRow.Cells[0].Text = "Email";
            grid.HeaderRow.Cells[0].Width = 220;
            grid.HeaderRow.Cells[1].Text = "Phone Day";
            grid.HeaderRow.Cells[1].Width = 100;
            grid.HeaderRow.Cells[2].Text = "Phone Night";
            grid.HeaderRow.Cells[2].Width = 100;
            grid.HeaderRow.Cells[3].Text = "User Name";
            grid.HeaderRow.Cells[3].Width = 220;
            grid.HeaderRow.Cells[4].Text = "First Name";
            grid.HeaderRow.Cells[4].Width = 100;
            grid.HeaderRow.Cells[5].Text = "Last Name";
            grid.HeaderRow.Cells[5].Width = 100;
            grid.HeaderRow.Cells[6].Text = "Title";
            grid.HeaderRow.Cells[6].Width = 250;
            grid.HeaderRow.Cells[7].Text = "Company";
            grid.HeaderRow.Cells[7].Width = 200;
            grid.HeaderRow.Cells[8].Text = "Location";
            grid.HeaderRow.Cells[8].Width = 100;
            grid.HeaderRow.Cells[9].Text = "Address Line 1";
            grid.HeaderRow.Cells[9].Width = 250;
            grid.HeaderRow.Cells[10].Text = "Address Line 2";
            grid.HeaderRow.Cells[10].Width = 250;
            grid.HeaderRow.Cells[11].Text = "City";
            grid.HeaderRow.Cells[11].Width = 100;
            grid.HeaderRow.Cells[12].Text = "State";
            grid.HeaderRow.Cells[12].Width = 40;
            grid.HeaderRow.Cells[13].Text = "Zip";
            grid.HeaderRow.Cells[13].Width = 40;
            grid.HeaderRow.Cells[14].Text = "Status";
            grid.HeaderRow.Cells[14].Width = 40;

            for (var i = 0; i <= 14; i++)
            {
                grid.HeaderRow.Cells[i].Font.Bold = false;
                grid.HeaderRow.Cells[i].VerticalAlign = VerticalAlign.Bottom;
            }

            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=ActiveUsersReport" + DateTime.Now + ".xls");
            Response.ContentType = "application/ms-excel";
            Response.Charset = "";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            //Line of code below fixes the issue with the date to be treated as date instead of text. This way we can leave the date as mm/dd/yy
            string style = @"<style> TD { mso-number-format:\@; } </style> ";
            Response.Write(style);
            grid.RenderControl(htw);
            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();
            return View("View");
        }
    }
}