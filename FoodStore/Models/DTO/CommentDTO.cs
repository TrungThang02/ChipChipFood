using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FoodStore.Models.DTO
{
    public class commentDTO
    {
        public int parentId { get; set; }
        public string commentText { get; set; }
        public string username { get; set; }
        public int productId { get; set; }

    }

    public class commentViewModel : commentDTO
    {
        public int commentId { get; set; }

        public DateTime createdDate { get; set; }
        public string strcreatedDate { get {; return this.createdDate.ToString("dd-MM-yyyy"); } }
    }

}