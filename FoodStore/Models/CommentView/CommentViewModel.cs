using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FoodStore.Models.CommentView
{
    public class CommentViewModel
    {
        public long ID { get; set; }
        public string CommentMsg { get; set; }
        public Nullable<System.DateTime> CommentDate { get; set; }
        public long ProductId { get; set; }
        public long CustomerId { get; set; }
        public string CustomerName { get; set; }
        public long ParentID { get; set; }
        public int Rate { get; set; }
    }
}