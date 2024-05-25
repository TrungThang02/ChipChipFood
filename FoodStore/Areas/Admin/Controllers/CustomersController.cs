using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using FoodStore.Models;

namespace FoodStore.Areas.Admin.Controllers
{
    public class CustomersController : Controller
    {
        private FoodStoreEntities db = new FoodStoreEntities();

        // GET: Admin/Customers
        public ActionResult Index()
        {
            if (Session["Admin"] == null)
            {

                return RedirectToAction("DangNhap", "Home");
            }
            else
            {
                return View(db.Customers.ToList());
            }
        }

        // GET: Admin/Customers/Details/5
        public ActionResult Details(int? id)
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

        // GET: Admin/Customers/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Admin/Customers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CustomerId,CustomerName,Address,BirthDay,UserName,Password,Email,Phone,ResetPasswordCode")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                db.Customers.Add(customer);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(customer);
        }

        // GET: Admin/Customers/Edit/5
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

        // POST: Admin/Customers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CustomerId,CustomerName,Address,BirthDay,UserName,Password,Email,Phone,ResetPasswordCode")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                db.Entry(customer).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(customer);
        }

        // GET: Admin/Customers/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Customer customer = db.Customer.Find(id);
        //    if (customer == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(customer);
        //}

        // POST: Admin/Customers/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            Customer customer = db.Customers.Find(id);

            if (customer == null)
            {
                // Nếu không tìm thấy khách hàng
                return HttpNotFound();
            }

            try
            {
                db.Customers.Remove(customer);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (DbUpdateException ex)
            {
                // Kiểm tra lỗi liên quan đến ràng buộc khóa ngoại
                if (ex.InnerException != null && ex.InnerException.InnerException is SqlException sqlException)
                {
                    if (sqlException.Number == 547) // Mã lỗi SQL Server cho lỗi ràng buộc khóa ngoại
                    {
                        TempData["ErrorMessage"] = "Không thể xóa khách hàng này do có dữ liệu liên quan trong hệ thống.";
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Đã xảy ra lỗi khi xóa khách hàng.";
                    }
                }
                else
                {
                    TempData["ErrorMessage"] = "Đã xảy ra lỗi khi xóa khách hàng.";
                }

                // Trả về View với thông báo lỗi
                return RedirectToAction("Index");
            }
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
