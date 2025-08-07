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
    public class TransactionsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Transactions
        public ActionResult Index(string sortOrder, string searchString, int? page)
        {
            ViewBag.DateSortParm = String.IsNullOrEmpty(sortOrder) ? "date_desc" : "";
            ViewBag.PropertySortParm = sortOrder == "Property" ? "prop_desc" : "Property";

            var transactions = from t in db.Transactions
                               select t;

            if (!String.IsNullOrEmpty(searchString))
            {
                transactions = transactions.Where(t => t.Property.name.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "date_desc":
                    transactions = transactions.OrderByDescending(t => t.transactionDate);
                    break;
                case "Property":
                    transactions = transactions.OrderBy(t => t.Property.name);
                    break;
                case "prop_desc":
                    transactions = transactions.OrderByDescending(t => t.Property.name);
                    break;
                default:
                    transactions = transactions.OrderBy(t => t.transactionDate);
                    break;
            }

            int pageSize = 10;
            int pageNumber = (page ?? 1);
            return View(transactions.ToList());
        }

        // GET: Transactions/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = db.Transactions.Find(id);
            if (transaction == null)
            {
                return HttpNotFound();
            }

            var currentUserId = User.Identity.GetUserId();
            var currentUser = db.Users.Find(currentUserId);

            if (!currentUser.IsAdministrator && transaction.userId != currentUserId)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }

            return View(transaction);
        }

        // GET: Transactions/Create
        public ActionResult Create()
        {
            ViewBag.PropertyId = new SelectList(db.Properties, "id", "name");
            return View();
        }

        // POST: Transactions/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,propertyId,transactionDate")] Transaction transaction)
        {
            if (ModelState.IsValid)
            {
                transaction.userId = User.Identity.GetUserId();
                db.Transactions.Add(transaction);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.PropertyId = new SelectList(db.Properties, "id", "name", transaction.propertyId);
            return View(transaction);
        }

        // GET: Transactions/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = db.Transactions.Find(id);
            if (transaction == null)
            {
                return HttpNotFound();
            }

            var currentUserId = User.Identity.GetUserId();
            var currentUser = db.Users.Find(currentUserId);

            if (!currentUser.IsAdministrator && transaction.userId != currentUserId)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }
            ViewBag.PropertyId = new SelectList(db.Properties, "id", "name", transaction.propertyId);
            return View(transaction);
        }

        // POST: Transactions/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,propertyId,userId,transactionDate")] Transaction transaction)
        {
            var currentUserId = User.Identity.GetUserId();
            var currentUser = db.Users.Find(currentUserId);
            var originalTransaction = db.Transactions.AsNoTracking().FirstOrDefault(t => t.id == transaction.id);

            if (originalTransaction == null)
            {
                return HttpNotFound();
            }

            if (!currentUser.IsAdministrator && originalTransaction.userId != currentUserId)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }

            if (ModelState.IsValid)
            {
                db.Entry(transaction).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.PropertyId = new SelectList(db.Properties, "id", "name", transaction.propertyId);
            return View(transaction);
        }

        // GET: Transactions/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = db.Transactions.Find(id);
            if (transaction == null)
            {
                return HttpNotFound();
            }

            var currentUserId = User.Identity.GetUserId();
            var currentUser = db.Users.Find(currentUserId);

            if (!currentUser.IsAdministrator && transaction.userId != currentUserId)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }
            return View(transaction);
        }

        // POST: Transactions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Transaction transaction = db.Transactions.Find(id);
            var currentUserId = User.Identity.GetUserId();
            var currentUser = db.Users.Find(currentUserId);

            if (!currentUser.IsAdministrator && transaction.userId != currentUserId)
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }

            db.Transactions.Remove(transaction);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
