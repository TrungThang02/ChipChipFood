
using FoodStore.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using FoodStore.helper;
using System.Configuration;
using System.Net.Mail;

namespace FoodStore.Controllers
{
    public class GioHangController : Controller
    {
        // GET: GioHang
        private FoodStoreEntities db = new FoodStoreEntities();


         public  List<GioHang> LayGioHang()
        {
            List<GioHang> lstGioHang = Session["GioHang"] as List<GioHang>;
            if (lstGioHang == null)
            {
                lstGioHang = new List<GioHang>();
                Session["GioHang"] = lstGioHang;
            }
            return lstGioHang;
        }



        public JsonResult ThemGioHang(int idproduct)
        {
            List<GioHang> lstGioHang = LayGioHang();

            GioHang sp = lstGioHang.Find(n => n.productId == idproduct);
            if (sp == null)
            {
                sp = new GioHang(idproduct);
                lstGioHang.Add(sp);
            }

            return Json(new { item = sp }, JsonRequestBehavior.AllowGet);
        }



        private int TongSoLuong()
        {
            int iTongSoLuong = 0;
            List<GioHang> lstGioHang = Session["GioHang"] as List<GioHang>;
            if (lstGioHang != null)
            {
                iTongSoLuong = lstGioHang.Sum(n => n.productQuantity);
            }
            return iTongSoLuong;
        }



        static private double dTongTien = 0;



        public double getTongTien()
        {
            return dTongTien + 30000;
        }
        public double TongTien()
        {
            List<GioHang> lstGioHang = Session["GioHang"] as List<GioHang>;
            if (lstGioHang != null)
            {
                dTongTien = lstGioHang.Sum(n => n.totalPrice);
            }
            return dTongTien;
        }
        public double TongTienHang()
        {
            List<GioHang> lstGioHang = Session["GioHang"] as List<GioHang>;
            if (lstGioHang != null)
            {
                dTongTien = lstGioHang.Sum(n => n.totalPrice);
            }
            return dTongTien;
        }

        public double PhiShip()
        {
            double PhiShip = 30000;
            return PhiShip;
        }



        public ActionResult GioHang()
        {
            List<GioHang> lstGioHang = LayGioHang();
            
            ViewBag.TongSoLuong = TongSoLuong();
            ViewBag.TongTien = TongTien();
            return View(lstGioHang);
        }



        public ActionResult GioHangPartial()
        {
            ViewBag.TongSoLuong = TongSoLuong();
            ViewBag.TongTien = TongTien();
            return PartialView();
        }



        public JsonResult CapNhatGioHang(int id, int quantity)     //truy cập sử dụng Url
        {
            List<GioHang> lstGioHang = LayGioHang();
            GioHang sp = lstGioHang.SingleOrDefault(n => n.productId == id);
            sp.productQuantity = quantity;
            return Json(new { item = sp }, JsonRequestBehavior.AllowGet);
        }



        public JsonResult XoaSPKhoiGioHang(int productId)
        {
            List<GioHang> lstGioHang = LayGioHang();
            GioHang sp = lstGioHang.SingleOrDefault(n => n.productId == productId);
            if (sp != null)
            {
                lstGioHang.RemoveAll(n => n.productId == productId);
                
            }
            return Json(new { item = sp }, JsonRequestBehavior.AllowGet);
        }




        public ActionResult XoaGioHang()
        {
            List<GioHang> lstGioHang = LayGioHang();
            lstGioHang.Clear();
            return RedirectToAction("Index", "SACHes");
        }



        [HttpGet]
        public ActionResult DatHang()
        {
            if (Session["TaiKhoan"] == null || Session["TaiKhoan"].ToString() == "")
            {
                return Redirect("~/User/DangNhap");
            }
            if (Session["GioHang"] == null)
            {
                return RedirectToAction("Index", "Products");
            }
            List<GioHang> lstGioHang = LayGioHang();
            ViewBag.TongSoLuong = TongSoLuong();
            ViewBag.TongTien = TongTien() + 30000 ;
            ViewBag.PhiShip = PhiShip();
            ViewBag.TongTienHang = TongTienHang();
            return View(lstGioHang);




        }



        [HttpPost]
        public ActionResult DatHang(FormCollection f)
        {
            var order = from d in db.Customers
                       join b in db.Orders
                       on d.CustomerId equals b.CustomerId
                       select new {
                           OrderDate = DateTime.Now,
                            address = b.Address,
                               mobile = d.Phone,
                            shipName = d.CustomerName,
                            email = d.Email

        };



            Product p = new Product();
            Order ddh = new Order();
            Customer kh = (Customer)Session["cmt"];
            List<GioHang> lstGioHang = LayGioHang();
            //.NullReferenceException
            if (kh.CustomerId != null)
            {
                try
                {
                    
                    ddh.CustomerId = kh.CustomerId;
                    
                    ddh.OrderDate = DateTime.Now;
                    ddh.Address = kh.Address;
                    ddh.RecipientPhone = kh.Phone;
               

                    var giatien = getTongTien();

                    ddh.OrderPrice = (decimal)giatien;


                    ddh.DeliveryDate = DateTime.Now.AddDays(3);

                    db.Orders.Add(ddh);
                    db.SaveChanges();
                }
                catch (NullReferenceException e)
                {
                    Console.WriteLine(e);
                }
            }

            OrderDetail ctdh = new OrderDetail();


            foreach (var item in lstGioHang)
            {
                ctdh.OrderId = ddh.OrderId;
                ctdh.ProductId = item.productId;
                ctdh.Quantity = item.productQuantity;
                ctdh.Price = (decimal)item.productPrice *item.productQuantity;
                db.OrderDetails.Add(ctdh);
            }
            var fromEmail = new MailAddress("chuhaist123@gmail.com", "ClothesShop");
            string content = System.IO.File.ReadAllText(Server.MapPath("~/Content/template/neworder.html"));
            string subject = "";
            subject = "Thông Báo";
            content = content.Replace("{{CustomerName}}", kh.CustomerName);
            content = content.Replace("{{Phone}}", kh.Phone);
            content = content.Replace("{{Email}}", kh.Email);
            content = content.Replace("{{Address}}", ddh.Address);
            var lstgiohang = LayGioHang();
            var noidung = "";
            foreach (var item in lstgiohang)
            {
                noidung += item.productName + ", ";
            }
            content = content.Replace("{{Product}}", noidung);
            content = content.Replace("{{Total}}", getTongTien().ToString("N0"));
            var toEmail = new MailAddress(kh.Email);

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = true,
                Credentials = new NetworkCredential(fromEmail.Address, "synmlgiwemgarjyu")
            };
            using (var message = new MailMessage(fromEmail, toEmail)
            {
                Subject = subject,
                Body = content,
                IsBodyHtml = true
            })
                smtp.Send(message);




            db.SaveChanges();

            Session["GioHang"] = null;
            return RedirectToAction("XacNhanDonHang", "GioHang");

           



        }

        public ActionResult ThanhToan()
        {
            return View();
        }

        public ActionResult XacNhanDonHang()
        {
            return View();
        }

        public ActionResult LichSuMuaHang(int? id)
        {
            var lsmh = from s in db.Orders where s.CustomerId == id select s;
            return View(lsmh);
        }

        public ActionResult Quanlidonhang(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            ViewBag.CustomerId = new SelectList(db.Customers, "CustomerId", "CustomerName", order.CustomerId);
            return View(order);
        }

        // POST: Admin/Orders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Quanlidonhang([Bind(Include = "OrderId,CustomerId,OrderDate,DeliveryDate,Address,Recipient,RecipientPhone,OrderState,OrderPrice")] Order order)
        {
            if (ModelState.IsValid)
            {
                db.Entry(order).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CustomerId = new SelectList(db.Customers, "CustomerId", "CustomerName", order.CustomerId);
            return View(order);
        }
    }
}
