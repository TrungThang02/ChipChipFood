using FoodStore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace FoodStore.Controllers
{
    
    
    public class HomeController : Controller
    {
        FoodStoreEntities db = new FoodStoreEntities();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Error()
        {
            return View();
        }
       public ActionResult NavbarPartial()
        {
           
            return PartialView();
        }
        public ActionResult NavigationPartial()
        {
            var danhmuc = from c in db.Categories select c;
            ViewBag.danhmuc = danhmuc;
            return PartialView();
        }
        public ActionResult BannerPartial()
        {
            return PartialView();
        }
        public ActionResult TopSellingPartial()

        {
            var p = from d in db.Products select d;
            return PartialView(p);
        }
        public ActionResult TabCategoryPartial()
        {
            return PartialView();
        }
        public ActionResult TimKiem(String search = "")
        {
            var dac = from d in db.Products select d;
            List<Product> products = db.Products.Where(p => p.ProductName.Contains(search)).ToList();
            ViewBag.search = search;
            return View(products);
        }
        public ActionResult About()
        {

            return View();
        }

        public ActionResult ContactPartial()
        {

            return PartialView();
        }


    }
}