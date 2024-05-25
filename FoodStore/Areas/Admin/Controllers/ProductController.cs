using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using FoodStore.Models;
using PagedList;

namespace FoodStore.Areas.Admin.Controllers
{
    public class ProductController : Controller
    {
        private FoodStoreEntities db = new FoodStoreEntities();

        // GET: Admin/Products
   

        public ActionResult Index(int? page)
        {
            if (Session["Admin"] == null)
            {
                
                return RedirectToAction("DangNhap", "Home");
            }
            else
            {
                int iSize = 5;
                int iPageNum = (page ?? 1);
                var product = db.Products.Include(p => p.Category).ToList();
                return View(product.OrderBy(x => x.ProductId).ToPagedList(iPageNum, iSize));
            }
            
        }

        // GET: Admin/Products/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        public ActionResult Create()
        {
            ViewBag.MaCD = new SelectList(db.Categories.ToList().OrderBy(n => n.CategoryName), "CategoryId", "CategoryName");
           
            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create(Product p, FormCollection f, HttpPostedFileBase fFileUpload)
        {
            //Đưa dữ liệu vào dropdown
            ViewBag.MaCD = new SelectList(db.Categories.ToList().OrderBy(n => n.CategoryName), "CategoryId", "CategoryName");

            if (fFileUpload == null)
            {
                //Thông báo yêu cầu chọn ảnh bìa
                ViewBag.ThongBao = "Hãy chọn ảnh !";
                //Lưu thông tin

                ViewBag.TenSach = f["sTenSach"];
                ViewBag.MoTa = f["sMoTa"];
                ViewBag.SoLuong = int.Parse(f["iSoLuong"]);
                ViewBag.DonGia = decimal.Parse(f["mDonGia"]);

                ViewBag.MaCD = new SelectList(db.Categories.ToList().OrderBy(n => n.CategoryName), "CategoryId", "CategoryName");
                

                return View();

            }
            else
            {
                if (ModelState.IsValid)
                {
                    //lấy tên file, khai báo thư viện(System IO)
                    var sFileName = Path.GetFileName(fFileUpload.FileName);
                    //Lấy đường dẫn lưu file
                    var path = Path.Combine(Server.MapPath("~/Content/Images"), sFileName);
                    //Kiểm tra ảnh đã được tải lên chưa
                    if (!System.IO.File.Exists(path))
                    {
                        fFileUpload.SaveAs(path);
                    }
                    //Lưu sách vào cơ sở dử liệu
                    p.ProductName = f["sTenSach"];
                    p.Description = f["sMoTa"].Replace("<p>", "").Replace("</p>", "\n");
                    p.Image = sFileName;

                    p.Count = int.Parse(f["iSoLuong"]);
                    p.Price = decimal.Parse(f["mDonGia"]);
                    p.CategoryId = int.Parse(f["MaCD"]);
                 
                    db.Products.Add(p);
                    db.SaveChanges();

                    return RedirectToAction("Index");

                }
                return View();
            }
        }
   
   
        [HttpGet]
        //public ActionResult Edit(int id)
        //{
        //    var sach = db.Product.SingleOrDefault(n => n.ProductId == id);
        //    if (sach == null)
        //    {
        //        Response.StatusCode = 404;
        //        return null;
        //    }
        //    else
        //    {
        //        ViewBag.MaCD = new SelectList(db.Category.ToList().OrderBy(n => n.CategoryName), "CategoryId", "CategoryName");


        //        return View(sach);
        //    }
        //}

  
        public ActionResult Edit(int id)
        {
            var sach = db.Products.SingleOrDefault(n => n.ProductId == id);
            if (sach == null)
            {
                Response.StatusCode = 404;
                return null;
            }
            else
            {
                ViewBag.MaCD = new SelectList(db.Categories.ToList().OrderBy(n => n.CategoryName), "CategoryId", "CategoryName");
             

                return View(sach);
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(FormCollection f, HttpPostedFileBase fFileUpload)
        {
            var sach = db.Products.AsEnumerable().SingleOrDefault(n => n.ProductId == int.Parse(f["iMaSach"]));
            ViewBag.MaCD = new SelectList(db.Categories, "CategoryId", "CategoryName", sach.CategoryId);


            if (ModelState.IsValid)
            {
                if (fFileUpload != null) //kiểm tra để xác nhận đổi ảnh
                {
                    var sFileName = Path.GetFileName(fFileUpload.FileName);
                    var path = Path.Combine(Server.MapPath("~/Content/Images"), sFileName);
                    //Kiểm tra ảnh đã được tải lên chưa
                        if (!System.IO.File.Exists(path))
                    {
                        System.IO.File.Delete(path);

                    }
                    else
                    {
                        fFileUpload.SaveAs(path);
                        sach.Image = sFileName;
                    }

                }

                //Lưu sách vào cơ sở dử liệu
                    sach.ProductName = f["sTenSach"];
                sach.Description = f["sMoTa"].Replace("<p>", "").Replace("<p>", "\n");



                sach.Price = decimal.Parse(f["mDonGia"]);


                db.SaveChanges();

                return RedirectToAction("Index");
            }
            return View(sach);



        }
        //public ActionResult Edit([Bind(Include = "ProductId,ProductName,Description,Image,CategoryId")] Product product)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(product).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    ViewBag.CategoryID = new SelectList(db.Category, "CategoryId", "CategoryName", product.CategoryId);

        //    return View(product);
        //}

        public ActionResult Delete(int id)
        {
            Product p = db.Products.FirstOrDefault(x => x.ProductId == id);
            db.Products.Remove(p);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        [HttpPost]
        public ActionResult DeleteSelected(int[] ids)
        {
            if (ids == null || ids.Length == 0)
            {
                return Json(new { success = false, message = "No items selected" });
            }

            try
            {
                var products = db.Products.Where(p => ids.Contains(p.ProductId)).ToList();
                foreach (var product in products)
                {
                    db.Products.Remove(product);
                }
                db.SaveChanges();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
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
