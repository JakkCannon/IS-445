using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SeatonFinalProject.Models;

namespace SeatonFinalProject.Controllers
{
    public class CustomersController : Controller
    {
        private ProjectDBEntities db = new ProjectDBEntities();
        int idInt;
        
        [HttpGet]
        public ActionResult Index()
        {
            int totRecords = db.Customers.Count();
            ViewBag.totRecords = totRecords;
            ViewBag.searchResults = -1;
            return View();
        }

        [HttpPost]
        public ActionResult Index(
            string idString,
            string firstName,
            string lastName,
            string state,
            string salesYTD,
            string pvSales)
        {
            int totRecords = db.Customers.Count();
            ViewBag.totRecords = totRecords;

            var customer = from c in db.Customers
                           select c;

            if (!String.IsNullOrEmpty(idString) && IsDigitsOnly(idString))
            {
                idInt = Int32.Parse(idString);
                customer = customer.Where(s => s.Id.Equals(idInt));
                ViewBag.searchResults = customer.Count();

                return View(customer);
            }

            if (!String.IsNullOrEmpty(firstName))
            {
                customer = customer.Where(s => s.CustFirstName.StartsWith(firstName));
            }

            if (!String.IsNullOrEmpty(lastName))
            {
                customer = customer.Where(s => s.CustLastName.StartsWith(lastName));
            }

            if (!String.IsNullOrEmpty(state))
            {
                customer = customer.Where(x => x.CustState.Equals(state));
            }

            if (!String.IsNullOrEmpty(salesYTD) && IsDigitsOnly(salesYTD))
            {
                int salesYTDint = Int32.Parse(salesYTD);
                customer = customer.Where(s => s.CustSalesYTD >= salesYTDint);
            }

            if (!String.IsNullOrEmpty(pvSales) && IsDigitsOnly(pvSales))
            {
                int pvSalesInt = Int32.Parse(pvSales);
                customer = customer.Where(s => s.CustSalesPrevious >= pvSalesInt);
            }

            ViewBag.searchResults = customer.Count();

            return View(customer);
        }

        bool IsDigitsOnly(string str)
        {
            foreach (char c in str)
            {
                if (c < '0' || c > '9')
                    return false;
            }
            return true;
        }

        // GET: Customers/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Customers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,CustFirstName,CustLastName,CustState,CustSalesYTD,CustSalesPrevious")] Customer customer, int? id)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    db.Customers.Add(customer);
                    db.SaveChanges();
                    ViewBag.CreateSuccess = "Customer created successfully.";
                    return View();
                }
            }
            catch (Exception)
            {
                ViewBag.UniqueIdError = "That ID already exists. Please enter a unique ID";
                return View();
            }
            return View(customer);
        }

        // GET: Customers/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = db.Customers.Find(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(customer);
        }

        // POST: Customers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,CustFirstName,CustLastName,CustState,CustSalesYTD,CustSalesPrevious")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                db.Entry(customer).State = EntityState.Modified;
                db.SaveChanges();
                ViewBag.EditSuccess = "Customer edited successfully";
                return View();
            }
            return View(customer);
        }

        // GET: Customers/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = db.Customers.Find(id);
            if (customer == null)
            {
                return HttpNotFound();
            }

            return View(customer);
        }

        // POST: Customers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Customer customer = db.Customers.Find(id);
            db.Customers.Remove(customer);
            db.SaveChanges();
            ViewBag.DeleteSuccess = "Customer successfully deleted";
            return View();
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
