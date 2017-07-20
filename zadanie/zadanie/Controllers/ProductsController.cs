using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using zadanie.Models;
using PagedList;
using Microsoft.Practices.Unity;
using System.Linq.Dynamic;
using zadanie.ViewModels;

namespace zadanie.Controllers
{
    public class ProductsController : Controller
    {
        private NorthwindEntities1 db;
        
        public ProductsController(NorthwindEntities1 db)
        {
            this.db = db;
        }
        // private NorthwindEntities1 db = new NorthwindEntities1();

        // GET: Products1
        public ActionResult Index(ProductViewModel model)
        {
            // Get Products
            model.Products = db.Products
                .Where(
                    x =>
                    (model.ProductName == null || x.ProductName.Contains(model.ProductName)))
                .OrderBy(model.Sort + " " + model.SortDir)
                .Skip((model.Page - 1) * model.PageSize)
                .Take(model.PageSize)
                .ToList();

            // total records for paging
            model.TotalRecords = db.Products
                .Count(x =>
                    (model.ProductName == null || x.ProductName.Contains(model.ProductName))
                    );

            return View(model);
        }

        // GET: Products1/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Products products = db.Products.Find(id);
            if (products == null)
            {
                return HttpNotFound();
            }
            return PartialView("Details", products);
        }

        // GET: Products1/Create
        public ActionResult Create()
        {
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName");
            ViewBag.SupplierID = new SelectList(db.Suppliers, "SupplierID", "CompanyName");
            return PartialView("Create");
        }

        // POST: Products1/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ProductID,ProductName,SupplierID,CategoryID,QuantityPerUnit,UnitPrice,UnitsInStock,UnitsOnOrder,ReorderLevel,Discontinued")] Products products)
        {
            if (ModelState.IsValid)
            {
                db.Products.Add(products);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName", products.CategoryID);
            ViewBag.SupplierID = new SelectList(db.Suppliers, "SupplierID", "CompanyName", products.SupplierID);
            return View(products);
        }

        // GET: Products1/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Products products = db.Products.Find(id);
            if (products == null)
            {
                return HttpNotFound();
            }
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName", products.CategoryID);
            ViewBag.SupplierID = new SelectList(db.Suppliers, "SupplierID", "CompanyName", products.SupplierID);
            return PartialView("Edit", products);
        }

        // POST: Products1/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ProductID,ProductName,SupplierID,CategoryID,QuantityPerUnit,UnitPrice,UnitsInStock,UnitsOnOrder,ReorderLevel,Discontinued")] Products products)
        {
            if (ModelState.IsValid)
            {
                db.Entry(products).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CategoryID = new SelectList(db.Categories, "CategoryID", "CategoryName", products.CategoryID);
            ViewBag.SupplierID = new SelectList(db.Suppliers, "SupplierID", "CompanyName", products.SupplierID);
            return View(products);
        }

        // GET: Products1/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Products products = db.Products.Find(id);
            if (products == null)
            {
                return HttpNotFound();
            }
            return PartialView("Delete", products);
        }

        // POST: Products1/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Products products = db.Products.Find(id);
            db.Products.Remove(products);
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
        public ActionResult List(string sortOrder, int? Page_No)
        {
            var products = from p in (db.Products.OrderBy(p => p.ProductName).Include(p => p.Categories).Include(p => p.Suppliers)) select p;

            if (!String.IsNullOrWhiteSpace(sortOrder))
            {
                if (sortOrder == "ProductName_ASC")
                    products = products.OrderBy(c => c.ProductName);
                else if (sortOrder == "ProductName_DESC")
                    products = products.OrderByDescending(c => c.ProductName);
                
            }
            else
            {
                products = products.OrderBy(c => c.ProductName);
            }

            int Size_Of_Page = 10;
            int No_Of_Page = (Page_No ?? 1);
            return View(products.ToPagedList(No_Of_Page, Size_Of_Page));
        }
    }
}
