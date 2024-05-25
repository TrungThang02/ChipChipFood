using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Web;

namespace FoodStore.Models.Mapping
{
    public class CommentMap : EntityTypeConfiguration<Comment>
    {
        public CommentMap()
        {
            // Primary Key
            this.HasKey(t => t.ID);


            // Table & Column Mappings
            this.ToTable("Comment");
            this.Property(t => t.ID).HasColumnName("ID");
            this.Property(t => t.CommentMsg).HasColumnName("CommentMsg");
            this.Property(t => t.CommentDate).HasColumnName("CommentDate");
            this.Property(t => t.ProductId).HasColumnName("ProductId");
            this.Property(t => t.CustomerId).HasColumnName("CustomerId");
            this.Property(t => t.ParentID).HasColumnName("ParentID");
            this.Property(t => t.Rate).HasColumnName("Rate");
        }
    }
}