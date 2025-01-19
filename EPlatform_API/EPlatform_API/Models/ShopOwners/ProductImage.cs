using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EPlatform_API.Models.ShopOwners
{
    public class ProductImage
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string? ImageId { get; set; }

        [Required]
        public int ProductId { get; set; }
        [Required]
        public string? ImgUrl { get; set; }

        public bool IsPrimary { get; set; }

        [Required]
        public string? ImgType { get; set; }
        public bool IsDeleted { get; set; }

        [Required]
        public DateTime UploadAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public Product? Product { get; set; }
    }
}