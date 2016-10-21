using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Mvc;
using AAAService.Models;

namespace AAAService.Controllers
{
    [Authorize(Roles = "SiteAdmin")]
    public class ExportToCFController : Controller
    {
        private aaahelpEntities db = new aaahelpEntities();

        // GET: ExportToCF
        public ActionResult Index()
        {
            var exportToCFData = from etcfd in db.ExportToCFs
                                 select etcfd;

            return View(exportToCFData.ToList<ExportToCF>());
        }

        public ActionResult ExportToExcel()
        {
            var exportToCFData = this.db.ExportToCFs.ToList();
            var grid = new GridView();
            grid.DataSource = exportToCFData;            
            grid.DataBind();
            grid.Font.Size = 8;
            grid.BorderStyle = BorderStyle.None;
            grid.Font.Name = "Arial";
            grid.HeaderRow.Cells[0].Text = "Job Number         (6)                      1a";
            grid.HeaderRow.Cells[0].Width = 69;            
            grid.HeaderRow.Cells[1].Text = "C/O EXT 1b";
            grid.HeaderRow.Cells[1].Width = 29;
            grid.HeaderRow.Cells[2].Text = "C/T  2";
            grid.HeaderRow.Cells[2].Width = 29;
            grid.HeaderRow.Cells[3].Text = "Job Date    3";
            grid.HeaderRow.Cells[3].Width = 64;
            grid.HeaderRow.Cells[4].Text = "Job Location Code (8)       6";
            grid.HeaderRow.Cells[4].Width = 61;
            //grid.HeaderRow.Cells[5].Text = "Cust Location No (3)        5";
            grid.HeaderRow.Cells[5].Text = "Cust" + Environment.NewLine + "Location No" + Environment.NewLine + "(3)" + Environment.NewLine + "5";            
            grid.HeaderRow.Cells[5].Width = 61;
            grid.HeaderRow.Cells[6].Text = "CUST. NO. (5)    4";
            grid.HeaderRow.Cells[6].Width = 45;
            grid.HeaderRow.Cells[7].Text = "JOB LOCATION LINE 1                                          (30)                                                                                                        7";
            grid.HeaderRow.Cells[7].Width = 245;
            grid.HeaderRow.Cells[8].Text = "JOB LOCATION LINE 2(30)                                                                                                         8";
            grid.HeaderRow.Cells[8].Width = 245;
            grid.HeaderRow.Cells[9].Text = "JOB LOCATION LINE 3(30)                                                                                                         9";
            grid.HeaderRow.Cells[9].Width = 245;
            grid.HeaderRow.Cells[10].Text = "JOB LOCATION LINE 4(30)                                                                                                         9";
            grid.HeaderRow.Cells[10].Width = 245;
            grid.HeaderRow.Cells[11].Text = "JOB REFERENCE                                                      (30)                                                                                                 11";
            grid.HeaderRow.Cells[11].Width = 245;
            grid.HeaderRow.Cells[12].Text = "JOB CONTACT                                                         (30)                                                                                                     12";
            grid.HeaderRow.Cells[12].Width = 245;
            grid.HeaderRow.Cells[13].Text = "PROJECT MANAGER       (15)                                                   13";
            grid.HeaderRow.Cells[13].Width = 125;
            grid.HeaderRow.Cells[14].Text = "CUSTOMER PO#             (15)                                                          14";
            grid.HeaderRow.Cells[14].Width = 125;
            grid.HeaderRow.Cells[15].Text = "SUBMITTED AMOUNT                                               15";
            grid.HeaderRow.Cells[15].Width = 125;
            grid.HeaderRow.Cells[16].Text = "C/O  Status                (1)                      16";
            grid.HeaderRow.Cells[16].Width = 85;
            grid.HeaderRow.Cells[17].Text = "STATUS DATE                            17";
            grid.HeaderRow.Cells[17].Width = 69;
            grid.HeaderRow.Cells[18].Text = "APPROVED   AMOUNT                           18";
            grid.HeaderRow.Cells[18].Width = 69;
            grid.HeaderRow.Cells[19].Text = "ESTIMATED TAX INCLUDED                               19";
            grid.HeaderRow.Cells[19].Width = 85;
            grid.HeaderRow.Cells[20].Text = "ESTIMATED TAX ADDITIONAL                                  20";
            grid.HeaderRow.Cells[20].Width = 85;
            grid.HeaderRow.Cells[21].Text = "RET. % (99.99)                  21";
            grid.HeaderRow.Cells[21].Width = 45;
            grid.HeaderRow.Cells[22].Text = "TAX STATE           (2)                22a";
            grid.HeaderRow.Cells[22].Width = 40;
            grid.HeaderRow.Cells[23].Text = "COUNTY      (10)                    22b";
            grid.HeaderRow.Cells[23].Width = 69;
            grid.HeaderRow.Cells[24].Text = "SALES REP            23";
            grid.HeaderRow.Cells[24].Width = 53;
            grid.HeaderRow.Cells[25].Text = "Sales Tax Y/N     24";
            grid.HeaderRow.Cells[25].Width = 37;
            grid.HeaderRow.Cells[26].Text = "Labor Tax Y/N     25";
            grid.HeaderRow.Cells[26].Width = 37;
            grid.HeaderRow.Cells[27].Text = "Use Tax Y/N     26";
            grid.HeaderRow.Cells[27].Width = 37;
            grid.HeaderRow.Cells[28].Text = "Condense  Costs          27";
            grid.HeaderRow.Cells[28].Width = 61;
            grid.HeaderRow.Cells[29].Text = "Job Complete  28";
            grid.HeaderRow.Cells[29].Width = 53;
            grid.HeaderRow.Cells[30].Text = "Alt G/L#   29";
            grid.HeaderRow.Cells[30].Width = 37;
            grid.HeaderRow.Cells[31].Text = "Std Phs/Cat      30";
            grid.HeaderRow.Cells[31].Width = 45;
            grid.HeaderRow.Cells[32].Text = "Markups    31";
            grid.HeaderRow.Cells[32].Width = 53;
            grid.HeaderRow.Cells[33].Text = "SORT DATE                            32";
            grid.HeaderRow.Cells[33].Width = 69;
            grid.HeaderRow.Cells[34].Text = "JOB START DATE                            33";
            grid.HeaderRow.Cells[34].Width = 69;
            grid.HeaderRow.Cells[35].Text = "JOB END DATE                            34";
            grid.HeaderRow.Cells[35].Width = 69;
            grid.HeaderRow.Cells[36].Text = "USER DEFINED 1              (15)                                       35";
            grid.HeaderRow.Cells[36].Width = 125;
            grid.HeaderRow.Cells[37].Text = "USER DEFINED 2              (15)                                       36";
            grid.HeaderRow.Cells[37].Width = 125;
            grid.HeaderRow.Cells[38].Text = "USER DEFINED 3              (15)                                       37";
            grid.HeaderRow.Cells[38].Width = 125;
            grid.HeaderRow.Cells[39].Text = "USER DEFINED 4              (15)                                       38";
            grid.HeaderRow.Cells[39].Width = 125;

            for (var i = 0; i <= 39; i++)
            {
                grid.HeaderRow.Cells[i].Font.Bold = false;
                grid.HeaderRow.Cells[i].VerticalAlign = VerticalAlign.Bottom;                
            }

            for (var i = 0; i <= grid.Rows.Count; i++)
            {
                grid.Rows[i].Cells[3].Text.num.Text.ToString("MM/DD/YY");
            }

            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=JJE" + DateTime.Now + ".xls");
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
    }    
}