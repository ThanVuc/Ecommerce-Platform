using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EPlatform_API.Models.Regions
{
    [Table("Districts")]
    public class District
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [MaxLength(20)]
        public string? Code { get; set; }

        [Required]
        [MaxLength(255)]
        public string? Name { get; set; }

        [MaxLength(255)]
        public string? NameEn { get; set; }

        [MaxLength(255)]
        public string? FullName { get; set; }

        [MaxLength(255)]
        public string? FullNameEn { get; set; }

        [MaxLength(255)]
        public string? CodeName {get; set;}

        [ForeignKey("Province")]
        public string? ProvinceCode { get; set; }
        public Province? Province { get; set; }

        [ForeignKey("AdministrativeUnit")]
        public int? AdministrativeUnitId { get; set; }
        public AdministrativeUnit? AdministrativeUnit { get; set; }
    }
}