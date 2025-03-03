using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace EPlatform_API.Models.ShopOwners
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }

        [ForeignKey("ParentCategory")]
        public int? CategoryParentId { get; set; }

        [Required]
        [StringLength(256)]
        public string? Name { get; set; }

        [Required]
        [StringLength(256)]
        public string? Slug { get; set; }

        public string? Description { get; set; }
        public string? ImgUrl { get; set; }
        public bool IsActive { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        [Required]
        public DateTime UpdatedAt { get; set; }
        [Required]
        public string? Code { get; set; }

        public Category? ParentCategory { get; set; }
        public ICollection<Product>? Products { get; set; }
        public ICollection<Category>? SubCategories { get; set; }

        public Stack<Category> getAllParentCategories()
        {
            Stack<Category> parentCategories = new Stack<Category>();
            Category? currentCategory = this;
            while (currentCategory != null)
            {
                parentCategories.Push(currentCategory);
                currentCategory = currentCategory.ParentCategory;
            }
            return parentCategories;
        }
    }
}