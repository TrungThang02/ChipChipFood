using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FoodStore.Models;

namespace FoodStore.Models.CommentView
{
    public class CommentDAO
    {

        FoodStoreEntities db = null;
        public CommentDAO()
        {
            db = new FoodStoreEntities();
        }
        public bool Insert(Comment entity)
        {
            db.Comments.Add(entity);
            db.SaveChanges();
            return true;
        }
        public List<Comment> ListComment(long parentId, long productId)
        {
            return db.Comments.Where(x => x.ParentID == parentId && x.ProductId == productId).ToList();
        }
        public List<CommentViewModel> ListCommentViewModel(long parentId, long productId)
        {
            var model = (from a in db.Comments
                         join b in db.Customers
                             on a.CustomerId equals b.CustomerId
                         where a.ParentID == parentId && a.ProductId == productId

                         select new
                         {
                             ID = a.ID,
                             CommentMsg = a.CommentMsg,
                             CommentDate = a.CommentDate,
                             ProductID = a.ProductId,
                             CustomerId = a.CustomerId,
                             CustomerName = b.CustomerName,
                             ParentID = a.ParentID,
                             Rate = a.Rate
                         }).AsEnumerable().Select(x => new CommentViewModel()
                         {
                             ID = x.ID,
                             CommentMsg = x.CommentMsg,
                             CommentDate = x.CommentDate,
                             ProductId = (long)x.ProductID,
                             CustomerId = (long)x.CustomerId,
                             CustomerName = x.CustomerName,
                             ParentID = (long)x.ParentID,
                             Rate = (int)x.Rate
                         });
            return model.OrderByDescending(y => y.ID).ToList();
        }
    }
}