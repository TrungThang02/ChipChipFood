using FoodStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using PagedList;
using FoodStore.Models.CommentView;
using PayPal.Api;
using System.Net.Mail;
using System.Net;
namespace FoodStore.Controllers
{
    public class ProductsController : Controller
    {
        FoodStoreEntities db = new FoodStoreEntities();

        // GET: Products
        public ActionResult Index(int? page)
        {

            int iSize = 9;
            int iPageNum = (page ?? 1);
         
            var dac = from d in db.Products select d;
            return View(dac.OrderBy(s => s.ProductId).ToPagedList(iPageNum, iSize));
        }
        public ActionResult ThucAnNhe()
        {
            int c = 1;
            var dc = from s in db.Categories
                     join p in db.Products on s.CategoryId equals p.CategoryId
                     where p.CategoryId == c
                     select p;
            return View(dc);

        }
        public ActionResult GaRan()
        {
            int c = 2;
            var dan = from s in db.Categories
                      join p in db.Products on s.CategoryId equals p.CategoryId
                      where p.CategoryId == c
                      select p;
            return View(dan);

        }
        public ActionResult Burger()
        {
            int c = 3;
            var du = from s in db.Categories
                     join p in db.Products on s.CategoryId equals p.CategoryId
                     where p.CategoryId == c
                     select p;
            return View(du);

        }
        public ActionResult TrangMieng()
        {
            int c = 4;
            var l = from s in db.Categories
                    join p in db.Products on s.CategoryId equals p.CategoryId
                    where p.CategoryId == c
                    select p;
            return View(l);

        }
        public ActionResult Combo()
        {
            int c = 5;
            var hs = from s in db.Categories
                     join p in db.Products on s.CategoryId equals p.CategoryId
                     where p.CategoryId == c
                     select p;
            return View(hs);

        }
        public ActionResult DoUong()
        {
            int c = 6;
            var hq = from s in db.Categories
                     join p in db.Products on s.CategoryId equals p.CategoryId
                     where p.CategoryId == c
                     select p;
            return View(hq);

        }


        public ActionResult ChiTIetSanPham(int id)
        {
            var ctsp = from s in db.Products where s.ProductId == id select s;
           
            var rate1 = (from s in db.Products
                         join c in db.Comments on s.ProductId equals c.ProductId
                         where c.Rate == 1
                         select c).Count();
            var rate2 = (from s in db.Products
                         join c in db.Comments on s.ProductId equals c.ProductId
                         where c.Rate == 2
                         select c).Count();
            var rate3 = (from s in db.Products
                         join c in db.Comments on s.ProductId equals c.ProductId
                         where c.Rate == 3
                         select c).Count();
            var rate4 = (from s in db.Products
                         join c in db.Comments on s.ProductId equals c.ProductId
                         where c.Rate == 4
                         select c).Count();
            var rate5 = (from s in db.Products join c in db.Comments on s.ProductId equals c.ProductId where c.Rate == 5 select c).Count();
            ViewBag.Rate3 = rate3;
            ViewBag.Rate1 = rate1;
            ViewBag.Rate2 = rate2;
            ViewBag.Rate4 = rate3;
            ViewBag.Rate5 = rate5;
            return View(ctsp);
        }
        public JsonResult AddNewComment(string commentmsg, int productid, string username, int parentid, string rate, int customerid)
        {

            using (FoodStoreEntities db = new FoodStoreEntities()) // Thay "YourDbContext" bằng tên thực tế của DbContext của bạn
            {
                Comment comment = new Comment
                {
                    CommentMsg = commentmsg,
                    CommentDate = DateTime.Now,

                    ProductId = productid,
                    UserName = username,

                    ParentID = parentid,
                    Rate = Convert.ToInt32(rate),
                    CustomerId = customerid,

                };

                db.Comments.Add(comment);
                db.SaveChanges();
                return Json(new { status = true }, JsonRequestBehavior.AllowGet);
            }
           

        }
        public ActionResult GetComment(int productid)
        {
            var data = new CommentDAO().ListCommentViewModel(0, productid);

            return PartialView("_ChildComment", data);
        }

        




    }


}
