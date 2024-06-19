using System;
using System.ComponentModel.DataAnnotations;

namespace DF_EvolutionAPI.Models
{

    public class Designation
    {
        [Key]
        public int DesignationId { get; set; }
        public int ReferenceId { get; set; }
        public string DesignationName { get; set; }
        public byte? IsActive { get; set; }
        public string? CreateBy { get; set; }
        public string? UpdateBy { get; set; }
        [Required]
        public DateTime CreateDate { get; set; }

        public DateTime? UpdateDate { get; set; }
    }
}
