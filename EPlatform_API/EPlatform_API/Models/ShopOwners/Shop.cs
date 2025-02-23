using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EPlatform_API.Models.ShopOwners
{
    public class Shop
    {
        [Key]
        // [StringLength(10)]
        [ForeignKey("ShopOwner")]
        public string? ShopId { get; set; }

        [Required]
        [StringLength(500)]
        public string? Name { get; set; }

        public string? Description { get; set; }
        public string? LogoUrl { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? Rating { get; set; }

        public int? ReviewCount { get; set; }
        public int? FollowersCount { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        [Required]
        [StringLength(500)]
        public string? Slug { get; set; }

        [Required]
        [StringLength(256)]
        public string? ShopAddress { get; set; }

        [Required]
        [StringLength(12)]
        public string? Phone { get; set; }

        [Required]
        [StringLength(256)]
        public string? Email { get; set; }
        public string? TaxesCode { get; set; }

        [Required]
        [StringLength(12)]
        public string? IdentificationNumber { get; set; }

        public ICollection<Product>? Products { get; set; }
        [Required]
        public AppUser? ShopOwner {get; set;}
    }
}