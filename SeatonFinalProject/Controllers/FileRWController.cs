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
    public class FileRWController : Controller
    {
        private ProjectDBEntities db = new ProjectDBEntities(); 

        [HttpGet]
        public ActionResult Export()
        {
            ViewBag.totRecords = db.Customers.Count();
            return View();
        }

        [HttpPost]
        public ActionResult Export(String fileName, String alsoDelete, String exportSales)
        {
            ViewBag.totRecords = db.Customers.Count();

            if (String.IsNullOrEmpty(fileName))
            {
                ViewBag.exportStatus = "File Name Empty. Try Again";
                return View();
            }
            StreamWriter sw = new StreamWriter(Server.MapPath("~/App_Data/" + fileName));

            if (String.IsNullOrEmpty(exportSales))
            {
                var customers = from c in db.Customers
                                orderby c.Id
                                select c;

                foreach (var item in customers)
                {
                    sw.WriteLine(item.Id + "~" + item.CustFirstName + "~" + item.CustLastName + "~" +
                    item.CustState + "~" + item.CustSalesYTD + "~" + item.CustSalesPrevious);

                    if (alsoDelete != null)
                    {
                        db.Customers.Remove(item);
                    }
                }
            }
            else
            {
                int exportCriteria = Convert.ToInt32(exportSales);

                var customers = from c in db.Customers
                                orderby c.Id
                                where c.CustSalesYTD <= (exportCriteria)
                                select c;

                foreach (var item in customers)
                {
                    sw.WriteLine(item.Id + "~" + item.CustFirstName + "~" + item.CustLastName + "~" +
                    item.CustState + "~" + item.CustSalesYTD + "~" + item.CustSalesPrevious);

                    if (alsoDelete != null)
                    {
                        db.Customers.Remove(item);
                    }
                }
            }
            
            db.SaveChanges();

            sw.Close();
            ViewBag.totRecords = db.Customers.Count(); 
            ViewBag.exportStatus = "Export Complete";
            return View();
        }

        [HttpGet]
        public ActionResult Import()
        {
            ViewBag.totRecords = db.Customers.Count();
            return View();
        }

        [HttpPost]
        public ActionResult Import(String fileName)
        {
            ViewBag.totRecords = db.Customers.Count();

            if (String.IsNullOrEmpty(fileName))
            {
                ViewBag.importStatus = "File Name Empty. Try Again";
                return View();
            }

            if (!System.IO.File.Exists(Server.MapPath("~/App_Data/" + fileName)))
            {
                ViewBag.importStatus = "File Does Not Exist. Try Again";
                return View();
            }

            StreamReader sr = new StreamReader(Server.MapPath("~/App_Data/" + fileName));

            string fileError = String.Format("{0}_{1}_{2:yyMMdd_HHmmss}{3}", fileName, "ERROR", DateTime.Now, ".txt");
            StreamWriter swError = new StreamWriter(Server.MapPath("~/App_Data/" + fileError));

            String[] customerRow;
            int recordsNotInserted = 0;
            int recordsInserted = 0;
            String fileLine = sr.ReadLine();

            while (fileLine != null)
            {

                customerRow = fileLine.Split('~');
                Customer customer = new Customer();
                customer.Id = Convert.ToInt32(customerRow[0]);
                
                if (db.Customers.Find(customer.Id) == null)
                {
                    customer.CustFirstName = customerRow[1];
                    customer.CustLastName = customerRow[2];
                    customer.CustState = customerRow[3];
                    customer.CustSalesYTD = Convert.ToInt32(customerRow[4]);
                    customer.CustSalesPrevious = Convert.ToInt32(customerRow[5]);
                    
                    db.Customers.Add(customer);
                    recordsInserted++;
                }
                
                else 
                {
                    recordsNotInserted++;

                    customer.CustFirstName = customerRow[1];
                    customer.CustLastName = customerRow[2];
                    customer.CustState = customerRow[3];
                    customer.CustSalesYTD = Convert.ToInt32(customerRow[4]);
                    customer.CustSalesPrevious = Convert.ToInt32(customerRow[5]);

                    swError.WriteLine(customer.Id + "~" + customer.CustFirstName + "~" + customer.CustLastName + "~" +
                    customer.CustState + "~" + customer.CustSalesYTD + "~" + customer.CustSalesPrevious);
                }

                fileLine = sr.ReadLine();
            }
           
            db.SaveChanges();
            sr.Close();
            swError.Close();

            ViewBag.recordsInserted = recordsInserted;
            ViewBag.recordsNotInserted = recordsNotInserted;

            ViewBag.totRecords = db.Customers.Count();
            return View();
        }

    }
}
