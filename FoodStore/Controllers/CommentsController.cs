using FoodStore.Models;
using FoodStore.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;


namespace FoodStore.Controllers
{
    public class CommentsController : Controller
    {

        FoodStoreEntities db = new FoodStoreEntities();
        CommentsRepo repo = new CommentsRepo();

        public ActionResult Index()
        {

            return PartialView();
        }

        public ActionResult CommentPartial()
        {
            string a = Request.Url.PathAndQuery;
            int b = Convert.ToInt32(a.Substring(25));





            var comments = (from c in db.Comments join p in db.Products on c.ProductId equals p.ProductId where c.ProductId == b select c).ToList();
            //var comments = repo.GetAll(productid);
            return PartialView("CommentPartial", comments);
        }

        public JsonResult AddNewComment(commentDTO comment)
        {
            try
            {


                var model = repo.AddComment(comment);

                return Json(new { error = false, result = model }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception)
            {
                //Handle Error here..
            }

            return Json(new { error = true }, JsonRequestBehavior.AllowGet);
        }
    }
}