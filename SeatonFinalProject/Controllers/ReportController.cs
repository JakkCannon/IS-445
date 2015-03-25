using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using SeatonFinalProject.Models;
using System.Text;

namespace SeatonFinalProject.Controllers
{
    public class ReportController : Controller
    {
        private ProjectDBEntities db = new ProjectDBEntities(); 

        // GET: Report
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(String report, String state, String sales)
        {
            if (report.Equals("1"))
            {
                var customers = from c in db.Customers
                                orderby c.CustLastName, c.CustFirstName
                                select c;
                ViewBag.reportStatus = "All Customers - Sorted by Customer Name";
                return View(customers);
            }

            if (report.Equals("2"))
            {
                var customers = from c in db.Customers
                                orderby (c.CustSalesYTD + c.CustSalesPrevious) descending
                                select c;
                ViewBag.reportStatus = "All Customers - Sorted by Current Total Sales Decreasing";
                return View(customers);
            }

            if (report.Equals("3"))
            {
                var customers = from c in db.Customers
                                orderby c.CustState, c.Id
                                select c;
                ViewBag.reportStatus = "Control Break Report - By State showing each States Total and Grand Total at End";
                return View("breakReport",customers);
            }

            if (report.Equals("4"))
            {
                var customers = from c in db.Customers
                                orderby c.CustSalesYTD, c.Id 
                                select c;

                ViewBag.reportStatus = "Customers with Current Year Total Sales Highest and Lowest Amounts";
                return View("reportMaxMin", customers);
            }

            if (report.Equals("5"))
            {
                if (String.IsNullOrEmpty(state))
                {
                    ViewBag.reportStatus = "You must enter a state";
                    return View();
                }

                int inSales;
                if (!int.TryParse(sales.Trim(), out inSales))
                {
                    ViewBag.reportStatus = "Rating must be an integer";
                    return View();
                }

                var customers = from c in db.Customers
                                orderby c.Id
                                where (c.CustState == state)
                                where ((c.CustSalesYTD + c.CustSalesPrevious) <= inSales)
                                select c;
                ViewBag.reportStatus = "Criteria Report - List Customers and Totals (This Year and Previous)";
                return View("criteriaReport",customers);
            }

            if (report.Equals("6"))
            {

                var customers = from c in db.Customers
                               orderby c.Id
                               select c;

                ViewBag.reportStatus = "Three Random Customers - Promotional Giveaway (Not Repeating)";
                return View("reportRandom", customers);
            }

            ViewBag.reportStatus = "No Reports Executed";
            return View();
        }
    }
}