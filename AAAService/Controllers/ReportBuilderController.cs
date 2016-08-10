using AAAService.Models;
using System;
using System.Data.Entity;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Data;
using System.Data.Linq;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Expressions;
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
            var now = DateTime.Now;
            reportInformation.From = DateTime.Now.Date;
            //reportInformation.From = new DateTime(2015, 11, 1).Date;
            reportInformation.To = new DateTime(now.Year, now.Month, now.Day, 23, 59, 59);

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

            var names = typeof(ReportBuilder_View).GetProperties()
                        .Select(property => property.Name)
                        .ToArray();

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

                    //var list = db.ReportBuilder_View.Where(rp => rp.order_datetime >= reportInformation.From && rp.order_datetime <= reportInformation.To).ToList();
                    //var select = list.Select(CreateNewStatement<ReportBuilder_View>(strDDLToValue));
                    //var result2 = GetColumn(list, strDDLToValue);

                    //var result = new List<ReportBuilder_View>();
                    //var data = new ReportBuilder_View();
                    //var type = data.GetType();
                    //var fieldName = "job_number";

                    //for (var i = 0; i < list.Count; i++)
                    //{
                    //    foreach (var property in data.GetType().GetProperties())
                    //    {
                    //        if (property.Name == fieldName)
                    //        {
                    //            type.GetProperties().FirstOrDefault(n => n.Name == property.Name).SetValue(data, GetPropValue(list[i], property.Name), null);
                    //            result.Add(data);
                    //        }
                    //    }
                    //}

                    //GenerateExcelSpreadsheet(strDDLToValueSplitted, result);
                    //var result = db.Database.SqlQuery(typeof< Database >, "SELECT " + strDDLToValue + " FROM ReportBuilder_View");

                    var sqlConnection = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString);
                    var sqlCommand = new SqlCommand();
                    SqlDataReader sqlDataReader;

                    sqlCommand.CommandText = "SELECT " + strDDLToValue + " FROM ReportBuilder_View WHERE order_datetime >= '" + reportInformation.From + "' And order_datetime <= '" + reportInformation.To + "'";

                    if (strDDLCompaniesValue != null)
                    {
                        sqlCommand.CommandText += " And parent_company_guid = '" + strDDLCompaniesValue + "'";
                    }

                    if (strDDLLocationsValue != null)
                    {
                        sqlCommand.CommandText += " And service_location_guid in (";
                        var strDDLLocationsValueSplitted = strDDLLocationsValue.Split(',');

                        foreach (var serviceLocationGuid in strDDLLocationsValueSplitted)
                        {
                            sqlCommand.CommandText += "'" + serviceLocationGuid + "', ";
                        }

                        sqlCommand.CommandText = sqlCommand.CommandText.Substring(0, sqlCommand.CommandText.Length - 2);
                        sqlCommand.CommandText += ")";
                    }

                    sqlCommand.CommandType = CommandType.Text;
                    sqlCommand.Connection = sqlConnection;

                    sqlConnection.Open();

                    sqlDataReader = sqlCommand.ExecuteReader();
                    var dataTable = new DataTable();
                    // Data is accessible through the DataReader object here.

                    if (sqlDataReader.HasRows)
                    {
                        //Console.WriteLine("\t{0}\t{1}", reader.GetName(0), reader.GetName(1));

                        //while (reader.Read())
                        //{
                        //    Console.WriteLine("{0}\t{1}", reader.GetInt32(0), reader.GetGuid(1));
                        //}

                        dataTable.Load(sqlDataReader);

                        Response.Clear();
                        Response.ClearHeaders();
                        Response.ClearContent();
                        
                        Response.ContentType = "text/csv";
                        Response.AddHeader("Content-Disposition", "attachment;filename=" + reportInformation.ReportName + ".csv;");
                        Response.AddHeader("Pragma", "no-cache");
                        Response.AddHeader("Expires", "0");

                        // 1. output columns
                        Boolean addComma = false;
                        Response.Write("\"");

                        foreach (DataColumn column in dataTable.Columns)
                        {
                            var columnHeader = db.ReportBuilderFields.Where(rp => rp.ColumnName == column.ColumnName).Select(rp => rp.Name).FirstOrDefault();

                            if (addComma)
                            {
                                Response.Write("\",\"");
                            }
                            else
                            {
                                addComma = true;
                            }

                            Response.Write(columnHeader);
                        } // foreach column

                        Response.Write("\"");
                        Response.Write(System.Environment.NewLine);

                        // 2. output data
                        foreach (DataRow row in dataTable.Rows)
                        {
                            addComma = false;
                            Response.Write("\"");

                            foreach (Object value in row.ItemArray)
                            {
                                // handle any embedded quotes.
                                String outValue = Convert.ToString(value).Replace("\"", String.Empty);

                                if (addComma)
                                {
                                    Response.Write("\",\"");
                                }
                                else
                                {
                                    addComma = true;
                                }

                                Response.Write(outValue);
                            }

                            Response.Write("\"");
                            Response.Write(System.Environment.NewLine);
                        } // foreach row

                        try
                        {
                            Response.Flush();
                            Response.End();
                        }
                        catch { }
                    }

                    sqlConnection.Close();                    
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

        public static object GetPropValue(object src, string propName)
        {
            return src.GetType().GetProperty(propName).GetValue(src, null);
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

        Func<T, T> CreateNewStatement<T>(string fields)
        {
            // input parameter "o"
            var xParameter = Expression.Parameter(typeof(T), "o");

            // new statement "new Data()"
            var xNew = Expression.New(typeof(T));

            // create initializers
            var bindings = fields.Split(',').Select(o => o.Trim())
                .Select(o => {

            // property "Field1"
            var mi = typeof(T).GetProperty(o);

            // original value "o.Field1"
            var xOriginal = Expression.Property(xParameter, mi);

            // set value "Field1 = o.Field1"
            return Expression.Bind(mi, xOriginal);
                }
            );

            // initialization "new Data { Field1 = o.Field1, Field2 = o.Field2 }"
            var xInit = Expression.MemberInit(xNew, bindings);

            // expression "o => new Data { Field1 = o.Field1, Field2 = o.Field2 }"
            var lambda = Expression.Lambda<Func<T, T>>(xInit, xParameter);

            // compile to Func<Data, Data>
            return lambda.Compile();
        }

        public IEnumerable<string> GetColumn(List<ReportBuilder_View> items, string columnName)
        {
            var values = items.Select(x => x.GetType().GetProperty(columnName).GetValue(x).ToString());
            return values;
        }
    }
}