using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace EPlatform_API.Models.Regions
{
    [Table("AdministrativeUnits")]
    public class AdministrativeUnit
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [MaxLength(255)]
        public string? FullName { get; set; }

        [MaxLength(255)]
        public string? FullNameEn { get; set; }

        [MaxLength(255)]
        public string? ShortName { get; set; }

        [MaxLength(255)]
        public string? ShortNameEn { get; set; }

        [MaxLength(255)]
        public string? CodeName { get; set; }

        [MaxLength(255)]
        public string? CodeNameEn { get; set; }
    }
}