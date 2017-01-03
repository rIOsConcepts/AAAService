using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using AAAService.Models;

namespace AAAService.Controllers
{
    public class ServiceCategoriesController : Controller
    {
        private aaahelpEntities db = new aaahelpEntities();

        // GET: ServiceCategories
        public ActionResult Index()
        {
            var serviceCategoryExtended = new List<ServiceCategoryExtended>();
            var serviceCategories = db.ServiceCategories;

            foreach (var serviceCategory in serviceCategories)
            {
                var data = db.Companies.Where(o => o.guid == serviceCategory.Company).Select(o => o.name);
                var companyName = "";

                if (data.Count() > 0)
                {
                    companyName = data.ToList()[0];
                }

                serviceCategoryExtended.Add(new ServiceCategoryExtended(serviceCategory)
                {                    
                    CompanyName = companyName                    
                });
            }

            serviceCategoryExtended = serviceCategoryExtended.OrderBy(o => o.CompanyName).ThenBy(o => o.ID).ToList();
            return View(serviceCategoryExtended);
        }

        // GET: ServiceCategories/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            
            ServiceCategory serviceCategory = db.ServiceCategories.Find(id);

            if (serviceCategory == null)
            {                
                return HttpNotFound();
            }

            var serviceCategoryExtended = new ServiceCategoryExtended(serviceCategory);
            var data = db.Companies.Where(o => o.guid == serviceCategory.Company).Select(o => o.name);
            var companyName = "";

            if (data.Count() > 0)
            {
                companyName = data.ToList()[0];
            }

            serviceCategoryExtended.CompanyName = companyName;
            return View(serviceCategoryExtended);
        }

        // GET: ServiceCategories/Create
        public ActionResult Create()
        {
            ViewBag.CompanyDropDown = new SelectList(db.Companies.Where(o => o.active == true).OrderBy(o => o.name), "guid", "name");
            return View();
        }

        // POST: ServiceCategories/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,Name,code,CfdataCode,OldCode,list_num,active,isAlert,AlertMessage")] ServiceCategory serviceCategory)
        {
            if (ModelState.IsValid)
            {
                serviceCategory.Company = Request.Form["CompanyDropDown"] != "" ? Guid.Parse(Request.Form["CompanyDropDown"]) : serviceCategory.Company;
                db.ServiceCategories.Add(serviceCategory);                
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            var serviceCategoryExtended = new ServiceCategoryExtended(serviceCategory);
            return View(serviceCategoryExtended);
        }

        // GET: ServiceCategories/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ServiceCategory serviceCategory = db.ServiceCategories.Find(id);

            if (serviceCategory == null)
            {
                return HttpNotFound();
            }

            var serviceCategoryExtended = new ServiceCategoryExtended(serviceCategory);
            var data = db.Companies.Where(o => o.guid == serviceCategory.Company).Select(o => o.name);
            var companyName = "";

            if (data.Count() > 0)
            {
                companyName = data.ToList()[0];
            }

            serviceCategoryExtended.CompanyName = companyName;
            ViewBag.Company = serviceCategoryExtended.Company;
            ViewBag.CompanyDropDown = new SelectList(db.Companies.Where(o => o.active == true).OrderBy(o => o.name), "guid", "name");
            return View(serviceCategoryExtended);
        }

        // POST: ServiceCategories/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,Name,code,CfdataCode,OldCode,list_num,active,isAlert,AlertMessage")] ServiceCategory serviceCategory)
        {
            if (ModelState.IsValid)
            {
                serviceCategory.Company = Request.Form["CompanyDropDown"] != "" ? Guid.Parse(Request.Form["CompanyDropDown"]) : serviceCategory.Company;
                db.Entry(serviceCategory).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(serviceCategory);
        }

        // GET: ServiceCategories/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            ServiceCategory serviceCategory = db.ServiceCategories.Find(id);

            if (serviceCategory == null)
            {
                return HttpNotFound();
            }

            var serviceCategoryExtended = new ServiceCategoryExtended(serviceCategory);
            var data = db.Companies.Where(o => o.guid == serviceCategory.Company).Select(o => o.name);
            var companyName = "";

            if (data.Count() > 0)
            {
                companyName = data.ToList()[0];
            }

            serviceCategoryExtended.CompanyName = companyName;
            return View(serviceCategoryExtended);
        }

        // POST: ServiceCategories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ServiceCategory serviceCategory = db.ServiceCategories.Find(id);
            db.ServiceCategories.Remove(serviceCategory);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
