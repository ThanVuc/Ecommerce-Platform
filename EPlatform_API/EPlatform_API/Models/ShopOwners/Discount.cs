using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EPlatform_API.Models.ShopOwners
{
    public class Discount
    {
        [Key]
        [StringLength(10)]
        public int DiscountId { get; set; }

        [Required]
        [StringLength(64)]
        public string? Code { get; set; }

        public string? Description { get; set; }

        [Required]
        [StringLength(20)]
        public string? DiscountType { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? DiscountValue { get; set; }

        [Required]
        public DateTime StartDay { get; set; }

        [Required]
        public DateTime EndDay { get; set; }

        public bool IsExpired { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? MinimumOrderValue { get; set; }

        public int? UsageLimit { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public ICollection<ProductDiscount>? ProductDiscounts { get; set; }
    }
}