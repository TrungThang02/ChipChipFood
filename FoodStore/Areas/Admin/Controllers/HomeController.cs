using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FoodStore.Models;
namespace FoodStore.Areas.Admin.Controllers
{
    public class HomeController : Controller
    {
        // GET: Admin/Home
        FoodStoreEntities db = new FoodStoreEntities();
        public ActionResult Index()
        {
            if (Session["Admin"] == null)
            {

                return RedirectToAction("DangNhap", "Home");
            }
            else
            {
                var tdh = (from t in db.Orders select t.OrderId).Count();
                ViewBag.TongDonHang = tdh;

                var ttk = (from c in db.Customers select c.CustomerId).Count();
                ViewBag.TongSoTaiKhoan = ttk;

                var tt = (from t in db.OrderDetails select t.Price).Sum();
                if(tt is null)
                {
                    ViewBag.TongTien = 0;

                }
                else
                {
                    ViewBag.TongTien = tt;
                }

                var tsp = (from p in db.Products select p.ProductId).Count();
                ViewBag.TongSoSanPham = tsp;
                return View();
            }
        }
 
      
        
        [HttpGet]
        public ActionResult DangNhap()
        {
            return View();
        }

        [HttpPost]
        public ActionResult DangNhap(FormCollection f)
        { 
            var sTenDN = f["Username"];
            var sMatKhau = f["Password"];
             ADMIN ad = db.ADMINs1.SingleOrDefault(n => n.UserName == sTenDN && n.Password == sMatKhau);
            if (ad != null)
            {
                Session["Admin"] = ad;
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.ThongBao = "Tên đăng nhập và mật khẩu không đúng !";
            }
            return View();
        }
        public ActionResult DangXuat()
        {
            Session.Remove("Admin");
            return RedirectToAction("Index", "Home");
        }

    }
}