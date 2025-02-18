using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EPlatform_API.Models.ShopOwners
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }

        [Required]
        [ForeignKey("Category")]
        public int CategoryId { get; set; }

        [Required]
        [ForeignKey("Shop")]
        public string? ShopId { get; set; }

        [Required]
        [StringLength(500)]
        public string? Name { get; set; }

        [Required]
        public string? Slug { get; set; }

        public string? AvtImgUrl { get; set; }
        public string? AvtImgName { get; set; }

        public string? Description { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? Price { get; set; }

        public bool IsPublic { get; set; }

        public int Likes { get; set; }
        public int Rate { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public Category? Category { get; set; }
        public Shop? Shop { get; set; }
        public Inventory? Inventory { get; set;}
        public ICollection<ProductDiscount>? ProductDiscounts { get; set; }
        public DateTime DeletedAt { get; set; }
        public bool IsDeleted { get; set; }
    }
}