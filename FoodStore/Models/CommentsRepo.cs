using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using FoodStore.Models;
using FoodStore.Models.CommentView;
using FoodStore.Models.DTO;

namespace FoodStore.Models
{
    public class CommentsRepo
    {
        FoodStoreEntities context = new FoodStoreEntities();

        public IQueryable<Comment> GetAll(int? productid)
        {

            return context.Comments.OrderBy(x => x.CommentDate).Where(x => x.ProductId == productid);
        }

        public CommentViewModel AddComment(commentDTO comment)
        {
            var _comment = new Comment()
            {
                ParentID = comment.parentId,
                CommentMsg = comment.commentText,
                UserName = comment.username,
                CommentDate = DateTime.Now,
                ProductId = comment.productId,
            };

            context.Comments.Add(_comment);
            context.SaveChanges();
            return context.Comments.Where(x => x.ID == _comment.ID)
                    .Select(x => new CommentViewModel
                    {
                        ID = x.ID,
                        CommentMsg = x.CommentMsg,
                        ParentID = (int)x.ParentID,
                        CommentDate = (DateTime)x.CommentDate,
                        CustomerName = x.UserName,
                        ProductId = (int)x.ProductId

                    }).FirstOrDefault();
        }
    }
}