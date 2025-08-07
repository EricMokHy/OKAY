using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using OKAY.Property.MVC.Models;
using Microsoft.AspNet.Identity;

namespace OKAY.Property.MVC.Controllers
{
    [Authorize]
    public class PropertiesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Properties
        public ActionResult Index(string sortOrder, string searchString, int? page)
        {
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.PriceSortParm = sortOrder == "Price" ? "price_desc" : "Price";

            var properties = from p in db.Properties
                           select p;

            if (!String.IsNullOrEmpty(searchString))
            {
                properties = properties.Where(p => p.name.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "name_desc":
                    properties = properties.OrderByDescending(p => p.name);
                    break;
                case "Price":
                    properties = properties.OrderBy(p => p.leasePrice);
                    break;
                case "price_desc":
                    properties = properties.OrderByDescending(p => p.leasePrice);
                    break;
                default:
                    properties = properties.OrderBy(p => p.name);
                    break;
            }

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(properties.ToList());
        }

        // GET: Properties/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Property @property = db.Properties.Find(id);
            if (@property == null)
            {
                return HttpNotFound();
            }

            var currentUserId = User.Identity.GetUserId();
            var currentUser = db.Users.Find(currentUserId);

            if (!currentUser.IsAdministrator && @property.userId != currentUserId)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }

            return View(@property);
        }

        // GET: Properties/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Properties/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,name,bedroom,isAvailable,leasePrice")] Property @property)
        {
            if (ModelState.IsValid)
            {
                @property.userId = User.Identity.GetUserId();
                db.Properties.Add(@property);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(@property);
        }

        // GET: Properties/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Property @property = db.Properties.Find(id);
            if (@property == null)
            {
                return HttpNotFound();
            }

            var currentUserId = User.Identity.GetUserId();
            var currentUser = db.Users.Find(currentUserId);

            if (!currentUser.IsAdministrator && @property.userId != currentUserId)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }
            return View(@property);
        }

        // POST: Properties/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,name,bedroom,isAvailable,leasePrice,userId")] Property @property)
        {
            var currentUserId = User.Identity.GetUserId();
            var currentUser = db.Users.Find(currentUserId);
            var originalProperty = db.Properties.AsNoTracking().FirstOrDefault(p => p.id == @property.id);

            if (originalProperty == null)
            {
                return HttpNotFound();
            }

            if (!currentUser.IsAdministrator && originalProperty.userId != currentUserId)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }

            if (ModelState.IsValid)
            {
                db.Entry(@property).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(@property);
        }

        // GET: Properties/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Property @property = db.Properties.Find(id);
            if (@property == null)
            {
                return HttpNotFound();
            }

            var currentUserId = User.Identity.GetUserId();
            var currentUser = db.Users.Find(currentUserId);

            if (!currentUser.IsAdministrator && @property.userId != currentUserId)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }
            return View(@property);
        }

        // POST: Properties/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Property @property = db.Properties.Find(id);
            var currentUserId = User.Identity.GetUserId();
            var currentUser = db.Users.Find(currentUserId);

            if (!currentUser.IsAdministrator && @property.userId != currentUserId)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }

            db.Properties.Remove(@property);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
