using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DF_EvolutionAPI.Models
{
   
    public class Designation
    {        
        [Key]
        public int DesignationId { get; set; }
        public int ReferenceId { get; set; }
        public string DesignationName { get; set; }
        public int? IsActive { get; set; }
        public int? CreateBy { get; set; }
        public int? UpdateBy { get; set; }
        [Required]
        public DateTime CreateDate { get; set; }

        public DateTime? UpdateDate { get; set; }
    }
}
