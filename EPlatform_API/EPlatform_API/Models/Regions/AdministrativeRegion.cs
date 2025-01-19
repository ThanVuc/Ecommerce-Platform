using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EPlatform_API.Models.Regions
{
    [Table("AdministrativeRegions")]
    public class AdministrativeRegion
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [Required]
        [MaxLength(255)]
        public string? Name { get; set; }

        [Required]
        [MaxLength(255)]
        public string? NameEn { get; set; }

        [MaxLength(255)]
        public string? CodeName { get; set; }

        [MaxLength(255)]
        public string? CodeNameEn { get; set; }
    }
}