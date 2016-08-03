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

        public ReportBuilderController(ApplicationUserManager userManager, ApplicationSignInManager signInManager, ApplicationRoleManager roleManager)
        {            
        }
        
        public async Task<ActionResult> Index()
        {
            var reportInformation = new Models.ReportBuilderModel.ReportInformation();
            reportInformation.ReportName = "Manuel Rios";
            reportInformation.From = DateTime.Now;
            reportInformation.To = DateTime.Now;

            var companies = db.Companies
                            .Where(c => c.active)
                            .Select(c => new Models.ReportBuilderModel.Companies()
                            {
                                GUID = c.guid,
                                Name = c.name
                            })
                            .OrderBy(c => c.Name);

            var locations = db.locationinfoes
                            .Where(l => l.active)
                            .Select(l => new Models.ReportBuilderModel.Locations()
                            {
                                GUID = l.guid,
                                Name = l.name
                            })
                            .OrderBy(l => l.Name);

            var reportBuilderFields = db.ReportBuilderFields
                            .Select(l => new Models.ReportBuilderModel.ReportBuilderFields()
                            {
                                Id = l.Id,
                                Name = l.Name
                            });

            return View(new Tuple<AAAService.Models.ReportBuilderModel.ReportInformation, IEnumerable<AAAService.Models.ReportBuilderModel.Companies>, IEnumerable<AAAService.Models.ReportBuilderModel.Locations>, IEnumerable<AAAService.Models.ReportBuilderModel.ReportBuilderFields>>(reportInformation, companies, locations, reportBuilderFields));
        }
    }
}